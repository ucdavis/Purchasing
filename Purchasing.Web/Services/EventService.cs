﻿using System;
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
        void OrderDenied(Order order, string comment);
        void OrderCancelled(Order order, string comment);
        void OrderCompleted(Order order);
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
        public void OrderApprovalAdded(Order order, Approval approval)
        {
            //var trackingEvent = new OrderTracking
            //{
            //    User = _userRepository.GetById(_userIdentity.Current),
            //    StatusCode = approval.StatusCode,
            //    Description = "routed"
            //};

            //order.AddTracking(trackingEvent);

            //_notificationService.OrderReRouted(order, order.StatusCode.Level);
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

        public void OrderDenied(Order order, string comment)
        {
            var user = _userRepository.GetById(_userIdentity.Current);
            
            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = "denied"
            };

            order.AddTracking(trackingEvent);

            _notificationService.OrderDenied(order, user, comment);
        }

        public void OrderCancelled(Order order, string comment)
        {
            var user = _userRepository.GetById(_userIdentity.Current);

            var trackingEvent = new OrderTracking
            {
                User = user,
                StatusCode = order.StatusCode,
                Description = "cancelled"
            };

            order.AddTracking(trackingEvent);

            _notificationService.OrderCancelled(order, user, comment);
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
                Description = "rerouted"
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
    }
}