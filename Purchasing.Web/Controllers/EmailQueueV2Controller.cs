

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using SendGridMail;
using SendGridMail.Transport;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// This is just for testing sending emails.
    /// </summary>
    [Authorize(Roles = Role.Codes.Admin)]
    public class EmailQueueV2Controller : ApplicationController
    {
	    private readonly IRepository<EmailQueueV2> _emailQueueV2Repository;

        public EmailQueueV2Controller(IRepository<EmailQueueV2> emailQueueV2Repository)
        {
            _emailQueueV2Repository = emailQueueV2Repository;
        }
    
        //
        // GET: /EmailQueueV2/
        public ActionResult Index()
        {
            var viewModel = EmailQueueV2ViewModel.Create();
            viewModel.EmailQueueV2List = _emailQueueV2Repository.Queryable.Where(a => (a.User == GetCurrentUser() || a.User == null) && a.Pending).ToList();
            viewModel.Orders = viewModel.EmailQueueV2List.Select(a => a.Order).Distinct().ToList();

            try
            {
                BatchEmail("jsylvestre@ucdavis.edu", viewModel.EmailQueueV2List);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
  
            

            return View(viewModel);
        }


        private void BatchEmail(string email, List<EmailQueueV2> pendingForUser)
        {
            var _sendGridUserName = ConfigurationManager.AppSettings["SendGridUserName"];
            var _sendGridPassword = ConfigurationManager.AppSettings["SendGridPassword"];

            Check.Require(!string.IsNullOrWhiteSpace(_sendGridPassword));
            Check.Require(!string.IsNullOrWhiteSpace(_sendGridUserName));


           // _emailQueueV2Repository.DbContext.BeginTransaction();
            var orders = pendingForUser.Select(a => a.Order).Distinct().ToList();

            var message = new StringBuilder();
            foreach (var order in orders)
            {
                var extraStyle1 = string.Empty;
                var extraStyle2 = string.Empty;
                if (order.StatusCode.Id == OrderStatusCode.Codes.Cancelled || order.StatusCode.Id == OrderStatusCode.Codes.Denied)
                {
                    extraStyle1 = "<span style=\"color: Red;\">";
                    extraStyle2 = "</span>";
                }
                message.Append("<p>");
                message.Append("<table>");
                message.Append("<tbody>");
                message.Append(string.Format("<tr><td style=\"width: 100px;\">Order Request</td><td>{0}</td></tr>", GenerateLink(order.RequestNumber)));
                message.Append(string.Format("<tr><td style=\"width: 100px;\"><strong>Created By:</strong></td><td>{0}</td></tr>", order.CreatedBy.FullName));
                message.Append(string.Format("<tr><td style=\"width: 100px;\"><strong>Status:</strong></td><td>{0}{1}{2}</td></tr>", extraStyle1, order.StatusCode.Name, extraStyle2));
                message.Append(string.Format("<tr><td style=\"width: 100px;\"><strong>Vendor:</strong></td><td>{0}</td></tr>", order.VendorName));


                message.Append("</tbody>");
                message.Append("</table>");
                

                message.Append("<table border=\"1\">");
                message.Append("<tbody>");

                foreach (var emailQueue in pendingForUser.Where(a => a.Order == order).OrderByDescending(b => b.DateTimeCreated))
                {
                    message.Append("<tr>");
                    message.Append(string.Format("<td style=\"padding-left: 7px; border-left-width: 0px; margin-left: 0px; width: 160px;\">{0}</td>", emailQueue.DateTimeCreated));
                    message.Append(string.Format("<td style=\"width: 137px;\">{0}</td>", emailQueue.Action));
                    message.Append(string.Format("<td >{0}</td>", emailQueue.Details));
                    message.Append("</tr>");

                    //emailQueue.Pending = false;
                    emailQueue.DateTimeSent = DateTime.Now;
                    _emailQueueV2Repository.EnsurePersistent(emailQueue);
                }

                message.Append("</tbody>");
                message.Append("</table>");
                message.Append("<hr>");
                message.Append("</p></br>");
            }


            var sgMessage = SendGrid.GenerateInstance();
            sgMessage.From = new MailAddress("opp-noreply@ucdavis.edu", "UCD PrePurchasing No Reply");

            sgMessage.Subject = pendingForUser.Count == 1
                        ? string.Format("PrePurchasing Notification for Order #{0}",
                                        pendingForUser.Single().Order.RequestNumber)
                        : "PrePurchasing Notifications";

            sgMessage.AddTo(email);
            sgMessage.Html = message.ToString();

            //var transport = SMTP.GenerateInstance(new NetworkCredential(_sendGridUserName, _sendGridPassword));
            //transport.Deliver(sgMessage);

            MailMessage message2 = new MailMessage("automatedemail@caes.ucdavis.edu", "jsylvestre@ucdavis.edu",
                                      sgMessage.Subject,
                                      sgMessage.Html);
            message2.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.ucdavis.edu");
            client.Send(message2);


           // _emailQueueV2Repository.DbContext.CommitTransaction();
        }

        private string GenerateLink(string orderRequestNumber)
        {
            return string.Format("<a href=\"{0}{1}\">{1}</a>", "http://prepurchasing.ucdavis.edu/Order/Lookup/", orderRequestNumber);
        }


    }

	/// <summary>
    /// ViewModel for the EmailQueueV2 class
    /// </summary>
    public class EmailQueueV2ViewModel
	{
        public List<EmailQueueV2> EmailQueueV2List { get; set; }
        public List<Order> Orders { get; set; } 
 
		public static EmailQueueV2ViewModel Create()
		{
			
			var viewModel = new EmailQueueV2ViewModel ();
 
			return viewModel;
		}
	}
}
