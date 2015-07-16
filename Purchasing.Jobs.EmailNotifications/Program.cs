using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure;
using Ninject;
using Purchasing.Core.Helpers;
using Purchasing.Core.Services;
using Purchasing.Jobs.Common;
using Purchasing.Jobs.Common.Logging;
using Microsoft.Azure.WebJobs;
using SendGrid;
using System.Net.Mail;

namespace Purchasing.Jobs.EmailNotifications
{
    public class Program : WebJobBase
    {
        private static IDbService _dbService;
        private static string _sendGridUserName;
        private static string _sendGridPassword;

        private const string SendGridFrom = "opp-noreply@ucdavis.edu";

        static void Main(string[] args)
        {
            LogHelper.ConfigureLogging();

            Console.WriteLine("Build Number: {0}", typeof(Program).Assembly.GetName().Version);

            var kernel = ConfigureServices();
            _dbService = kernel.Get<IDbService>();
            var jobHost = new JobHost();
            jobHost.Call(typeof(Program).GetMethod("EmailNotifications"));
        }

        [NoAutomaticTrigger]
        public static void EmailNotifications()
        {
            var sendEmail = CloudConfigurationManager.GetSetting("opp-send-email");

            //Don't execute unless email is turned on
            if (!string.Equals(sendEmail, "Yes", StringComparison.InvariantCultureIgnoreCase)) return;

            //Setup sendGrid info, so we only look it up once per execution call
            _sendGridUserName = CloudConfigurationManager.GetSetting("opp-sendgrid-username");
            _sendGridPassword = CloudConfigurationManager.GetSetting("opp-sendgrid-pass");
        }

        private void BatchEmail(IDbConnection connection, string email, List<dynamic> pendingForUser)
        {
            var pendingOrderIds = pendingForUser.Select(x => x.OrderId).Distinct();

            //Do batches inside of their own transactions
            using (var ts = connection.BeginTransaction())
            {
                var pendingOrders =
                    connection.Query(@"select o.Id, o.RequestNumber, o.OrderStatusCodeId
                                        ,u.FirstName + ' ' + u.LastName as CreatedByFullName, os.Name as StatusName
                                        ,wv.Name as VendorName, wv.Line1, wv.City, wv.State, wv.Zip, wv.CountryCode
                                    from Orders o inner join Users u on u.Id = o.CreatedBy 
                                        inner join OrderStatusCodes os on os.Id = o.OrderStatusCodeId
                                        left outer join WorkgroupVendors wv on wv.Id = o.WorkgroupVendorId
                                    where o.id in @ids",
                                new { ids = pendingOrderIds.ToArray() }, ts)
                              .ToList();

                var message = new StringBuilder();
                message.Append(string.Format("<p>{0}</p>", "Here is your summary for the PrePurchasing system."));
                foreach (var order in pendingOrders)
                {
                    var extraStyle1 = string.Empty;
                    var extraStyle2 = string.Empty;

                    if (order.OrderStatusCodeId == "OC" || order.OrderStatusCodeId == "OD") //cancelled or denied
                    {
                        extraStyle1 = "<span style=\"color: Red;\">";
                        extraStyle2 = "</span>";
                    }

                    message.Append("<p>");
                    message.Append("<table>");
                    message.Append("<tbody>");
                    message.Append(string.Format("<tr><td style=\"width: 100px;\">Order Request</td><td>{0}</td></tr>",
                                                 GenerateLink(order.RequestNumber)));
                    message.Append(
                        string.Format(
                            "<tr><td style=\"width: 100px;\"><strong>Created By:</strong></td><td>{0}</td></tr>",
                            order.CreatedByFullName));
                    message.Append(
                        string.Format(
                            "<tr><td style=\"width: 100px;\"><strong>Status:</strong></td><td>{0}{1}{2}</td></tr>",
                            extraStyle1, order.StatusName, extraStyle2));
                    message.Append(
                        string.Format("<tr><td style=\"width: 100px;\"><strong>Vendor:</strong></td><td>{0}</td></tr>",
                                      string.IsNullOrWhiteSpace(order.VendorName)
                                          ? "-- Unspecified --"
                                          : string.Format("{0} ({1}, {2}, {3} {4}, {5})", order.VendorName,
                                                          order.Line1, order.City, order.State, order.Zip,
                                                          order.CountryCode)));

                    message.Append("</tbody>");
                    message.Append("</table>");

                    message.Append("<table border=\"1\">");
                    message.Append("<tbody>");

                    dynamic orderid = order.Id;
                    foreach (var emailQueue in pendingForUser.Where(a => a.OrderId == orderid).OrderByDescending(b => b.DateTimeCreated))
                    {
                        message.Append("<tr>");
                        message.Append(string.Format("<td style=\"padding-left: 7px; border-left-width: 0px; margin-left: 0px; width: 180px;\">{0}</td>", emailQueue.DateTimeCreated));
                        message.Append(string.Format("<td style=\"width: 137px;\">{0}</td>", emailQueue.Action));
                        message.Append(string.Format("<td >{0}</td>", emailQueue.Details ?? string.Empty));
                        message.Append("</tr>");

                        //TODO: Can move to single update outside of foreach
                        connection.Execute("update EmailQueueV2 set Pending = 0, DateTimeSent = @now where id = @id",
                                           new { now = DateTime.Now.ToPacificTime(), id = emailQueue.Id }, ts);
                    }

                    message.Append("</tbody>");
                    message.Append("</table>");
                    message.Append("<hr>");
                    message.Append("</p></br>");
                }

                message.Append(string.Format("<p><em>{0} </em><em><a href=\"{1}\">{2}</a>&nbsp;</em></p>", "You can change your email preferences at any time by", "http://prepurchasing.ucdavis.edu/User/Profile", "updating your profile on the PrePurchasing site"));

                var sgMessage = new SendGridMessage
                {
                    From = new MailAddress(SendGridFrom, "UCD PrePurchasing No Reply"),
                    Subject = pendingOrders.Count == 1
                        ? String.Format((string) "PrePurchasing Notification for Order #{0}",
                            new[] {pendingOrders.Single().RequestNumber})
                        : "PrePurchasing Notifications"
                };

                sgMessage.AddTo(email);
                sgMessage.Html = message.ToString();

                var transportWeb = new Web(new NetworkCredential(_sendGridUserName, _sendGridPassword));
                transportWeb.Deliver(sgMessage);

                ts.Commit();
            }
        }

        private string GenerateLink(string orderRequestNumber)
        {
            return string.Format("<a href=\"{0}{1}\">{1}</a>", "http://prepurchasing.ucdavis.edu/Order/Lookup/", orderRequestNumber);
        }
    }
}
