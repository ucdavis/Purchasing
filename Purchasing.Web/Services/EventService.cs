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
    }

    public class EventService : IEventService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;

        public EventService(IUserIdentity userIdentity, IRepositoryWithTypedId<User, string> userRepository)
        {
            _userIdentity = userIdentity;
            _userRepository = userRepository;
        }

        public void OrderApprovalAdded(Order order, Approval approval){}

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
                StatusCode = order.StatusCode,
                Description = "created"
            };

            order.AddTracking(trackingEvent);
        }

        public void OrderStatusChange(Order order, OrderStatusCode newStatusCode){ }
    }
}