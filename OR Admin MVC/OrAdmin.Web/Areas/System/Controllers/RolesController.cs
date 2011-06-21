using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MvcMembership;
using OrAdmin.Core.Attributes.Authorization;
using OrAdmin.Core.Enums.App;
using OrAdmin.Core.Extensions;
using OrAdmin.Entities.App;
using OrAdmin.Web.Areas.System.Models.Roles;

namespace OrAdmin.Web.Areas.System.Controllers
{
    [MasterAdmin]
    public class RolesController : BaseController
    {
        private const int PageSize = int.MaxValue;
        private readonly IRolesService _rolesService;
        private readonly IUserService _userService;

        public RolesController()
            : this(
                new AspNetMembershipProviderWrapper(Membership.Provider),
                new AspNetRoleProviderWrapper(Roles.Provider))
        {
        }

        public RolesController(
            IUserService userService,
            IRolesService rolesService)
        {
            _userService = userService;
            _rolesService = rolesService;
        }

        public ViewResult Index()
        {
            return View(new IndexViewModel { Roles = _rolesService.FindAll().Select(r => new Role() { RoleName = r }) });
        }

        public ViewResult Details(string id)
        {
            return View(new RoleViewModel
            {
                Role = id,
                Users = _rolesService.FindUserNamesByRole(id).Select(username => _userService.Get(username))
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult CreateRole(string id)
        {
            try
            {
                _rolesService.Create(id);
                TempData[GlobalProperty.Message.SuccessMessage.ToString()] = "Role successfully added!";
            }
            catch (Exception ex)
            {
                TempData[GlobalProperty.Message.FailMessage.ToString()] = "Role could not be added. " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult DeleteRole(string id)
        {
            _rolesService.Delete(id);
            TempData[GlobalProperty.Message.SuccessMessage.ToString()] = "Role successfully deleted!";
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult AddToRole(Guid id, string role)
        {
            _rolesService.AddToRole(_userService.Get(id), role);
            TempData[GlobalProperty.Message.SuccessMessage.ToString()] = String.Format("User added to {0}!", role);
            return RedirectToAction("Details", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult RemoveFromRole(Guid id, string role)
        {
            _rolesService.RemoveFromRole(_userService.Get(id), role);
            TempData[GlobalProperty.Message.SuccessMessage.ToString()] = String.Format("User removed from {0}!", role);
            return RedirectToAction("Details", new { id });
        }
    }
}
