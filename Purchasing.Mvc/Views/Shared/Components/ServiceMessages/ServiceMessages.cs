using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Mvc.Controllers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Attributes;
using Purchasing.Mvc;

namespace Purchasing.Mvc.Components
{
    public class ServiceMessages : ViewComponent
    {
        private const string CacheKey = "ServiceMessage";

        private readonly IRepository<ServiceMessage> _serviceMessageRepository;
        private readonly IMemoryCache _cache;

        public ServiceMessages(IRepository<ServiceMessage> serviceMessageRepository, IMemoryCache cache)
        {
            _serviceMessageRepository = serviceMessageRepository;
            _cache = cache;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var serviceMessageList = _cache.GetOrCreate(CacheKey, entry =>
            {
                entry.AbsoluteExpiration = DateTime.UtcNow.ToPacificTime().Date.AddDays(1);
                using (var ts = new TransactionScope())
                {
                    var currentDate = DateTime.UtcNow.ToPacificTime().Date;
                    var serviceMessageListToCache = _serviceMessageRepository.Queryable
                        .Where(a => a.IsActive && a.BeginDisplayDate <= currentDate && (a.EndDisplayDate == null || a.EndDisplayDate >= currentDate))
                        .ToList();
                    ts.CommitTransaction();
                    return serviceMessageListToCache;
                }
            });

            return Task.FromResult<IViewComponentResult>(View(serviceMessageList));
        }
    }
}