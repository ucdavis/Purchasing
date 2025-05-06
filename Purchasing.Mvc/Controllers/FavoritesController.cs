using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Mvc.App_GlobalResources;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Services;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Purchasing.WS;
using Serilog;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Purchasing.Mvc.Helpers;
using Purchasing.Core.Services;
using AggieEnterpriseApi.Validation;
using NHibernate.Util;

namespace Purchasing.Mvc.Controllers
{

    public class FavoritesController : ApplicationController
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueryRepositoryFactory _queryRepository;

        public FavoritesController(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepository)
        {
            _repositoryFactory = repositoryFactory;
            _queryRepository = queryRepository;
        }

        public IActionResult Index()
        {
            var user = _repositoryFactory.UserRepository.GetById(CurrentUser.Identity.Name);

            var favorites = _repositoryFactory.FavoriteRepository.Queryable
                .Where(a => a.User.Id == user.Id && a.IsActive)
                .Select(a => new FavoriteViewModel
                {
                    Id = a.Id,
                    OrderId = a.Order.Id,
                    Category = a.Category,
                    Notes = a.Notes,
                    RequestNumber = a.Order.RequestNumber,
                    Status = a.Order.StatusCode.Name,
                    WorkgroupName = a.Order.Workgroup.Name,
                    DateAdded = a.DateAdded
                })
                .ToList();

            return View(favorites);
        }

        [HttpPost]
        public JsonNetResult ToggleFavorite(int orderId, bool favorite, string category, string notes)
        {
            var fav = _repositoryFactory.FavoriteRepository.Queryable
                .Where(x => x.User.Id == CurrentUser.Identity.Name && x.Order.Id == orderId)
                .SingleOrDefault();
            var order = _repositoryFactory.OrderRepository.GetById(orderId);
            if (order == null)
            {
                throw new Exception("Order Not Found");
            }
            if (fav == null)
            {
                if (favorite)
                {
                    fav = new Favorite
                    {
                        Category = category,
                        Notes = notes,
                        User = _repositoryFactory.UserRepository.GetNullableById(CurrentUser.Identity.Name),
                        Order = order,
                        IsActive = true,
                        DateAdded = DateTime.UtcNow
                    };
                }
                else
                {
                    return new JsonNetResult(new { success = false });
                }
            }
            else
            {

                if (fav.IsActive)
                {
                    fav.IsActive = favorite;
                    fav.Category = category;
                    fav.Notes = notes;
                }
                else
                {
                    fav.IsActive = favorite;
                    fav.DateAdded = DateTime.UtcNow;
                    fav.Category = category;
                    fav.Notes = notes;
                }
            }

            _repositoryFactory.FavoriteRepository.EnsurePersistent(fav);

            return new JsonNetResult(new { success = true, category = fav.Category, notes = fav.Notes, isActive = fav.IsActive });
        }

        public ActionResult Delete(int id)
        {
            var favorite = _repositoryFactory.FavoriteRepository.GetNullableById(id);
            if (favorite == null)
            {
                return RedirectToAction("Index");
            }
            if (favorite.User.Id != CurrentUser.Identity.Name)
            {
                return RedirectToAction("Index");
            }
            favorite.IsActive = false;
            _repositoryFactory.FavoriteRepository.EnsurePersistent(favorite);
            return RedirectToAction("Index");
        }

    }
}
