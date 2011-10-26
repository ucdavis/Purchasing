using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using System.Security.Principal;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Services
{
    public interface IEventService
    {
        void OrderApproved(Order order, Approval approval);
        void OrderStatusChange(Order order, OrderStatusCode newStatusCode);
        void OrderApprovalAdded(Order order, Approval approval);
        void OrderCreated(Order order);
        void OrderAutoApprovalAdded(Order order, Approval approval);
        void OrderReRouted(Order order);
        void OrderEdited(Order order);
    }

    public class EventService : IEventService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<OrderStatusCode, string> _orderStatusCodeRepository;

        public EventService(IUserIdentity userIdentity, IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<OrderStatusCode,string> orderStatusCodeRepository)
        {
            _userIdentity = userIdentity;
            _userRepository = userRepository;
            _orderStatusCodeRepository = orderStatusCodeRepository;
        }

        public void OrderApprovalAdded(Order order, Approval approval){}

        public void OrderAutoApprovalAdded(Order order, Approval approval)
        {
            var trackingEvent = new OrderTracking
                                    {
                                        User = approval.User,
                                        StatusCode = approval.StatusCode,
                                        Description = "automatically approved" //TODO: what info do we want here?
                                    };

            order.AddTracking(trackingEvent);
        }

        public void OrderApproved(Order order, Approval approval)
        {
            var trackingEvent = new OrderTracking
                                    {
                                        User = _userRepository.GetById(_userIdentity.Current),
                                        StatusCode = approval.StatusCode,
                                        Description = "approved"
                                    };

            order.AddTracking(trackingEvent);
        }

        public void OrderCreated(Order order)
        {
            var trackingEvent = new OrderTracking
            {
                User = _userRepository.GetById(_userIdentity.Current),
                StatusCode = _orderStatusCodeRepository.GetById(OrderStatusCode.Codes.Requester),
                Description = "created"
            };

            order.AddTracking(trackingEvent);
        }

        public void OrderReRouted(Order order)
        {
            var trackingEvent = new OrderTracking
            {
                User = _userRepository.GetById(_userIdentity.Current),
                StatusCode = order.StatusCode,
                Description = "rerouted"
            };

            order.AddTracking(trackingEvent);
        }

        public void OrderEdited(Order order)
        {
            var trackingEvent = new OrderTracking
            {
                User = _userRepository.GetById(_userIdentity.Current),
                StatusCode = order.StatusCode,
                Description = "edited"
            };

            order.AddTracking(trackingEvent);
        }

        public void OrderStatusChange(Order order, OrderStatusCode newStatusCode){ }
    }
}