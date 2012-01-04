using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using AutoMapper;

namespace Purchasing.Web.Services
{
    public interface IArchiveOrderService
    {
        void ArchiveOrder(Order order);
    }


    public class ArchiveOrderService : IArchiveOrderService
    {
        public void ArchiveOrder (Order order)
        {

        }
    }
}