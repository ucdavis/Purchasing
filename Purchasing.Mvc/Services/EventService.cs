using Purchasing.Core.Domain;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Mvc.Services
{
    public interface IEventService
    {
        void OrderApproved(Order order, Approval approval);
        void OrderApprovedByAdmin(Order order, Approval approval);
        void OrderStatusChange(Order order, OrderStatusCode newStatusCode);
        void OrderApprovalAdded(Order order, Approval approval, bool notify = false);
        void OrderCreated(Order order);
        void OrderAutoApprovalAdded(Order order, Approval approval);
        void OrderReRouted(Order order);
        void OrderEdited(Order order);
        void OrderDenied(Order order, string comment, OrderStatusCode previousStatus);
        void OrderCancelled(Order order, string comment, OrderStatusCode previousStatus);
        void OrderCompleted(Order order);
        void OrderReceived(Order order, LineItem lineItem, decimal quantity, string overrideDescription = null);
        void OrderPaid(Order order, LineItem lineItem, decimal quantity, string overrideDescription = null);
        void OrderReRoutedToPurchaser(Order order, string routedTo);
        void OrderReRoutedToAccountManager(Order order, string external, string originalRouting, string routedTo);

        void OrderUpdated(Order order, string whatWasUpdated);

        void OrderAddAttachment(Order order);
        void OrderAddNote(Order order, string comment);
    }

    public class EventService : IEventService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;
        private readonly INotificationService _notificationService;

        public EventService(IUserIdentity userIdentity, IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<OrderStatusCode,string> orderStatusCodeRepository, INotificationService notificationService)
        {
            _userIdentity = userIdentity;
            _userRepository = userRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Order approval is added to the order
        /// </summary>
        /// <remarks>
        /// Current doesn't do anything, do not need to do the tracking on adding approvals.
        /// </remarks>
        /// <param name="order"></param>
        /// <param name="approval"></param>
        /// <param name="notify"> </param>
        public void OrderApprovalAdded(Order order, Approval approval, bool notify = false)
        {
            //var trackingEvent = new OrderTracking
            //{
            //    User = _userRepository.GetById(_userIdentity.Current),
            //    StatusCode = approval.StatusCode,
            //    Description = "routed"
            //};

            //order.AddTracking(trackingEvent);
            if(notify)
            {
                _notificationService.OrderReRouted(order, order.StatusCode.Level, approval.User != null);
            }
        }

        public void OrderAutoApprovalAdded(Order order, Approval approval)
        {
            //_notificationService.OrderApproved(order, approval);

            var trackingEvent = new OrderTracking
                                    {
                                        User = approval.User,
                                        StatusCode = approval.StatusCode,
                                        Description = "automatically approved" //TODO: what info do we want here?
                                    };

            order.AddTracking(trackingEvent);
        }

        /// <summary>
        /// Call notification service, THEN add the tracking.
        /// </summary>
        /// <param name="order">Order's status is at the current level</param>
        /// <param name="approval">Approval is at the current level, and completed is true</param>
        public void OrderApproved(Order order, Approval approval)
        {
            var trackingEvent = new OrderTracking
                                    {
                                        User = _userRepository.GetById(_userIdentity.Current),
                                        StatusCode = approval.StatusCode,
                                        Description = "approved"
                                    };

            order.AddTracking(trackingEvent);

            _notificationService.OrderApproved(order, approval);
        }

        /// <summary>
        /// Same as OrderApproved, except Description is different
        /// </summary>
        /// <param name="order"></param>
        /// <param name="approval"></param>
        public void OrderApprovedByAdmin(Order order, Approval approval)
        {
            var trackingEvent = new OrderTracking
            {
                User = _userRepository.GetById(_userIdentity.Current),
                StatusCode = approval.StatusCode,
                Description = "approved (Admin Override)"
            };

            order.AddTracking(trackingEvent);

            _notificationService.OrderApproved(order, approval);
        }

        public void OrderDenied(Order order, string comment, OrderStatusCode previousStatus)
        {
            var user = _userRepository.GetById(_userIdentity.Current);
            
            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = "denied"
            };

            order.AddTracking(trackingEvent);

            _notificationService.OrderDenied(order, user, comment, previousStatus);
        }

        public void OrderCancelled(Order order, string comment, OrderStatusCode previousStatus)
        {
            var user = _userRepository.GetById(_userIdentity.Current);

            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = "cancelled"
            };

            order.AddTracking(trackingEvent);

            _notificationService.OrderCancelled(order, user, comment, previousStatus);
        }

        public void OrderCompleted(Order order)
        {
            var user = _userRepository.GetById(_userIdentity.Current);

            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = "completed"
            };

            order.AddTracking(trackingEvent);

            _notificationService.OrderCompleted(order, user);
        }

        public void OrderReceived(Order order, LineItem lineItem, decimal quantity, string overrideDescription = null)
        {
            var user = _userRepository.GetById(_userIdentity.Current);
            var description = overrideDescription;
            if (string.IsNullOrWhiteSpace(description))
            {
                description = string.Format("{0} received {1} of {2}", user.FullName, quantity, lineItem.Description);
            }

            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = description
            };

            order.AddTracking(trackingEvent);
            if (string.IsNullOrWhiteSpace(overrideDescription))
            {
                _notificationService.OrderReceived(order, lineItem, user, quantity);
            }
            else
            {
                _notificationService.OrderReceived(order, lineItem, user, quantity, string.Format("{0} by {1}", description, user.FullName));
            }
        }

        public void OrderPaid(Order order, LineItem lineItem, decimal quantity, string overrideDescription = null)
        {
            var user = _userRepository.GetById(_userIdentity.Current);
            var description = overrideDescription;
            if (string.IsNullOrWhiteSpace(description))
            {
                description = string.Format("{0} paid {1} of {2}", user.FullName, quantity, lineItem.Description);
            }

            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = description
            };

            order.AddTracking(trackingEvent);

            if (string.IsNullOrWhiteSpace(overrideDescription))
            {
                _notificationService.OrderPaid(order, lineItem, user, quantity);
            }
            else
            {
                _notificationService.OrderPaid(order, lineItem, user, quantity, string.Format("{0} by {1}", description, user.FullName));
            }
        }

        public void OrderReRoutedToPurchaser(Order order, string routedTo)
        {
            var user = _userRepository.GetById(_userIdentity.Current);
            
            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = string.Format("rerouted to purchaser {0}", routedTo)
            };

            order.AddTracking(trackingEvent);
        }

        public void OrderReRoutedToAccountManager(Order order, string external, string originalRouting, string routedTo)
        {
            var user = _userRepository.GetById((_userIdentity.Current));

            var trackingEvent = new OrderTracking
                {
                    User = user,
                    StatusCode = order.StatusCode,
                    Description = string.Format("{0}approval routed to {1} from {2}", external, routedTo, originalRouting)
                };
            order.AddTracking(trackingEvent);
        }

        public void OrderUpdated(Order order, string whatWasUpdated)
        {
            var user = _userRepository.GetById((_userIdentity.Current));
            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = whatWasUpdated
            };
            order.AddTracking(trackingEvent);
        }

        public void OrderCreated(Order order)                      
        {
            order.GenerateRequestNumber();

            var trackingEvent = new OrderTracking
            {
                User = _userRepository.GetById(_userIdentity.Current),
                StatusCode = _orderStatusCodeRepository.GetById(OrderStatusCode.Codes.Requester),
                Description = "created"
            };

            order.AddTracking(trackingEvent);

            _notificationService.OrderCreated(order);
        }

        public void OrderReRouted(Order order)
        {
            var trackingEvent = new OrderTracking
            {
                User = _userRepository.GetById(_userIdentity.Current),
                StatusCode = order.StatusCode,
                Description = "edited & rerouted"
            };

            order.AddTracking(trackingEvent);

            _notificationService.OrderReRouted(order, order.StatusCode.Level);
        }

        public void OrderEdited(Order order)
        {
            var user = _userRepository.GetById(_userIdentity.Current);

            _notificationService.OrderEdited(order, user);

            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = "edited"
            };

            order.AddTracking(trackingEvent);
        }

        public void OrderStatusChange(Order order, OrderStatusCode newStatusCode){ }

        public void OrderAddAttachment(Order order)
        {
            var user = _userRepository.GetById(_userIdentity.Current);
            _notificationService.OrderAddAttachment(order, user);
        }

        public void OrderAddNote(Order order, string comment)
        {
            var user = _userRepository.GetById(_userIdentity.Current);
            _notificationService.OrderAddNote(order, user, comment);
        }
    }
}