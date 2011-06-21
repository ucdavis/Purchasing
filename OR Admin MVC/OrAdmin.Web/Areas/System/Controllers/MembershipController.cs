using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using MvcMembership;
using OrAdmin.Core.Attributes.Authorization;
using OrAdmin.Core.Enums.App;
using OrAdmin.Core.Extensions;
using OrAdmin.Web.Areas.System.Models.Membership;

namespace OrAdmin.Web.Areas.System.Controllers
{
    [MasterAdmin]
    public class MembershipController : BaseController
    {
        private const int PageSize = int.MaxValue;
        private readonly IRolesService _rolesService;
        private readonly IUserService _userService;

        public MembershipController()
            : this(
                new AspNetMembershipProviderWrapper(Membership.Provider),
                new AspNetRoleProviderWrapper(Roles.Provider))
        {
        }

        public MembershipController(
            IUserService userService,
            IRolesService rolesService)
        {
            _userService = userService;
            _rolesService = rolesService;
        }

        public ViewResult Index(int? index)
        {
            return View(new IndexViewModel { Users = _userService.FindAll(index ?? 0, PageSize) });
        }

        public ViewResult Details(Guid id)
        {
            var user = _userService.Get(id);
            var userRoles = _rolesService.FindByUser(user);
            return View(new DetailsViewModel
                            {
                                DisplayName = User.Profile(user.UserName).FirstName + " " + User.Profile(user.UserName).LastName,
                                User = user,
                                Roles = _rolesService.FindAll().ToDictionary(role => role, role => userRoles.Contains(role)),
                                Status = user.IsOnline
                                            ? DetailsViewModel.StatusEnum.Online
                                            : !user.IsApproved
                                                ? DetailsViewModel.StatusEnum.Unapproved
                                                : user.IsLockedOut
                                                    ? DetailsViewModel.StatusEnum.LockedOut
                                                    : DetailsViewModel.StatusEnum.Offline
                            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult Details(Guid id,
                                    [Bind(Prefix = "User.Email")] string email,
                                    [Bind(Prefix = "User.Comment")] string comment)
        {
            var user = _userService.Get(id);
            user.Email = email;
            user.Comment = comment;
            _userService.Update(user);
            TempData[GlobalProperty.Message.SuccessMessage.ToString()] = "Profile successfully updated!";
            return RedirectToAction("details", new { id });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult DeleteUser(Guid id)
        {
            _userService.Delete(_userService.Get(id));
            TempData[GlobalProperty.Message.SuccessMessage.ToString()] = "User account deleted!";
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RedirectToRouteResult ChangeApproval(Guid id, bool isApproved)
        {
            var user = _userService.Get(id);
            user.IsApproved = isApproved;
            _userService.Update(user);
            TempData[GlobalProperty.Message.SuccessMessage.ToString()] = String.Format("Profile successfully {0}!", isApproved ? "approved" : "un-approved");
            return RedirectToAction("Details", new { id });
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