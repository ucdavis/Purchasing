using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Create an email queue object for an approval decision
        /// </summary>
        /// <param name="order">Order being updated</param>
        /// <param name="user">User performing the approval action</param>
        /// <param name="role">The current role performing the approval action</param>
        /// <param name="approved">Whether to send a approved or denied message</param>
        /// <param name="targetUser">User to be notified</param>
        void ApprovalDecision(Order order, User user, Role role, bool approved, User targetUser, string notificationType);

        //void KualiUpdate(Order order);
    }

    public class NotificationService : INotificationService
    {
        /* strings to be used in the messages */
        private const string ApprovalMessage = "Order request #{0}, has been {2} by {3} for {4} review.";

        /* private helper properties */
        private readonly IRepository<EmailQueue> _emailQueueRepository;

        public NotificationService(IRepository<EmailQueue> emailQueueRepository)
        {
            _emailQueueRepository = emailQueueRepository;
        }

        public void ApprovalDecision(Order order, User user, Role role, bool approved, User targetUser, string notificationType)
        {
            Check.Require(order != null, "order is required.");
            Check.Require(user != null, "user is required.");
            Check.Require(role != null, "role is required.");
            Check.Require(targetUser != null , "targetUser is required.");

            var txt = string.Format(ApprovalMessage, order.Id, approved ? "approved" : "denied", user.FullName, role.Name);

            var emailQueue = new EmailQueue(order, notificationType, txt, user);

            _emailQueueRepository.EnsurePersistent(emailQueue);
        }
    }
}