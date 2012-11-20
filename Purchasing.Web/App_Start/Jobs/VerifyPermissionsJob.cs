using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Web.Services;
using Quartz;
using SendGridMail;
using SendGridMail.Transport;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.App_Start.Jobs
{
    public class VerifyPermissionsJob : IJob
    {
        private readonly IRepository<Workgroup> _workgroupRepository;
        private readonly IRepository<WorkgroupPermission> _workgroupPermissionRepository;
        private readonly IQueryRepositoryFactory _queryRepositoryFactory;

        public VerifyPermissionsJob()
        {
            _workgroupRepository = ServiceLocator.Current.GetInstance<IRepository<Workgroup>>();
            _workgroupPermissionRepository = ServiceLocator.Current.GetInstance<IRepository<WorkgroupPermission>>();
            _queryRepositoryFactory = ServiceLocator.Current.GetInstance<IQueryRepositoryFactory>();
            _queryRepositoryFactory.RelatatedWorkgroupsRepository = ServiceLocator.Current.GetInstance<IRepository<RelatedWorkgroups>>();
        }

        public void Execute(IJobExecutionContext context)
        {
            var sendGridUserName = context.MergedJobDataMap["sendGridUser"] as string;
            var sendGridPassword = context.MergedJobDataMap["sendGridPassword"] as string;
            try
            {
                var workgroupService = new WorkgroupService(null, null, null, null, _workgroupPermissionRepository, null, null, null, null, _queryRepositoryFactory, null);

                var childWorkGroups = _workgroupRepository.Queryable.Where(a => a.IsActive && !a.Administrative);
                foreach (var childWorkGroup in childWorkGroups)
                {
                    var parentWorkGroupIds = workgroupService.GetParentWorkgroups(childWorkGroup.Id);
                    var parentPermissions = _workgroupPermissionRepository.Queryable.Where(a => parentWorkGroupIds.Contains(a.Workgroup.Id)).Select(s => new { s.Id, role = s.Role.Id, user = s.User.Id, parentWorkgroupId = s.Workgroup.Id, s.Workgroup.IsFullFeatured }).ToList();
                    var childPermissions = _workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == childWorkGroup && a.IsAdmin).Select(s => new { s.Id, role = s.Role.Id, user = s.User.Id, parentWorkgroupId = s.ParentWorkgroup.Id, s.IsFullFeatured }).ToList();

                    var missingChildPermissions = parentPermissions.Where(a => !childPermissions.Any(b => b.role == a.role && b.user == a.user && b.parentWorkgroupId == a.parentWorkgroupId && b.IsFullFeatured == a.IsFullFeatured)).ToList();
                    var extraChildPermissions = childPermissions.Where(a => !parentPermissions.Any(b => b.role == a.role && b.user == a.user && b.parentWorkgroupId == a.parentWorkgroupId && b.IsFullFeatured == a.IsFullFeatured)).ToList();

                    if (missingChildPermissions.Count > 0 || extraChildPermissions.Count > 0)
                    {
                        //Send message that there is a problem
                        SendSingleEmail("jsylvestre@ucdavis.edu", "PrePurchasing Permissions Error", string.Format("Missing Child Permission count: {0}<br />Extra Child Permission count: {1}", missingChildPermissions.Count, extraChildPermissions.Count), sendGridUserName, sendGridPassword);
                    }
                    else
                    {
                        //Send message AOK
                        SendSingleEmail("jsylvestre@ucdavis.edu", "PrePurchasing Permissions Ok", string.Format("Missing Child Permission count: {0}<br />Extra Child Permission count: {1}", missingChildPermissions.Count, extraChildPermissions.Count), sendGridUserName, sendGridPassword);
                    }
                }
            }
            catch (Exception ex)
            {
                SendSingleEmail("jsylvestre@ucdavis.edu", "PrePurchasing Permissions Exception", string.Format("Exception occurred: {0}", ex.Message), sendGridUserName, sendGridPassword);
                throw;
            }
        }

        private void SendSingleEmail(string email, string subject, string body, string sendGridUserName, string sendGridPassword)
        {
            Check.Require(!string.IsNullOrWhiteSpace(sendGridUserName));
            Check.Require(!string.IsNullOrWhiteSpace(sendGridPassword));

            var sgMessage = SendGrid.GenerateInstance();
            sgMessage.From = new MailAddress("opp-noreply@ucdavis.edu", "OPP No Reply");
            sgMessage.Subject = subject;
            sgMessage.AddTo(email);
            sgMessage.Html = body;

            var transport = SMTP.GenerateInstance(new NetworkCredential(sendGridUserName, sendGridPassword));
            transport.Deliver(sgMessage);
        }
    }
}