using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Controllers
{
    /// <summary>
    /// Controller for the Admin class
    /// </summary>
    public class AdminController : ApplicationController
    {
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private readonly IRepositoryWithTypedId<Role, string> _roleRepository;
        private readonly IRepositoryWithTypedId<Organization, string> _organizationRepository;

        public AdminController(IRepositoryWithTypedId<User, string> userRepository, IRepositoryWithTypedId<Role, string> roleRepository, IRepositoryWithTypedId<Organization,string> organizationRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _organizationRepository = organizationRepository;
        }

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            var admins = _roleRepository.Queryable.Where(x => x.Name.EndsWith("Admin")).Fetch(x => x.Users).ToList();

            var model = new AdminListModel()
                            {
                                Admins = admins.Single(x => x.Id == Role.Codes.Admin).Users.Where(x=>x.IsActive).ToList(),
                                DepartmentalAdmins = admins.Single(x => x.Id == Role.Codes.DepartmentalAdmin).Users.Where(x=>x.IsActive).ToList()
                            };

            return View(model);
        }

        public ActionResult ModifyDepartmental(string id)
        {
            var user = _userRepository.Queryable.Where(x => x.Id == id).Fetch(x => x.Organizations).SingleOrDefault() ??
                       new User(null) {IsActive = true};

            var model = new DepartmentalAdminModel
                            {
                                User = user,
                                Organizations = _organizationRepository.Queryable.Where(x => x.TypeCode == "D").ToList()//TODO: For now, just get the full department types
                            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ModifyDepartmental(DepartmentalAdminModel departmentalAdminModel)
        {
            if (!ModelState.IsValid)
            {
                departmentalAdminModel.Organizations = _organizationRepository.Queryable.Where(x => x.TypeCode == "D").ToList();//TODO: For now, just get the full department types
                return View(departmentalAdminModel);
            }

            var user = _userRepository.GetNullableById(departmentalAdminModel.User.Id) ?? new User();

            departmentalAdminModel.User.Roles = user.Roles;

            Mapper.Map(departmentalAdminModel.User, user);

            var isDeptAdmin = user.Roles.Any(x => x.Id == Role.Codes.DepartmentalAdmin);
            
            if (!isDeptAdmin)
            {
                user.Roles.Add(_roleRepository.GetById(Role.Codes.DepartmentalAdmin));
            }

            _userRepository.EnsurePersistent(user);

            Message = string.Format("{0} was added as a departmental admin to the specified organization(s)",
                                    user.FullNameAndId);

            return RedirectToAction("Index");
        }

        public ActionResult ModifyAdmin(string id)
        {
            var user = _userRepository.GetNullableById(id) ?? new User(null) {IsActive = true};
            
            return View(user);
        }

        [HttpPost]
        public ActionResult ModifyAdmin(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var userToSave = _userRepository.GetNullableById(user.Id) ?? new User();
            user.Organizations = userToSave.Organizations; //Transfer the orgs and roles since they aren't managed on this page
            user.Roles = userToSave.Roles;

            Mapper.Map(user, userToSave);

            var isAdmin = userToSave.Roles.Any(x => x.Id == Role.Codes.Admin);

            if (!isAdmin)
            {
                userToSave.Roles.Add(_roleRepository.GetById(Role.Codes.Admin));
            }
            
            _userRepository.EnsurePersistent(userToSave);

            Message = string.Format("{0} was added to the administrator role", user.FullNameAndId);

            return RedirectToAction("Index");
        }
    }

    public class DepartmentalAdminModel
    {
        public User User { get; set; }
        public virtual IEnumerable<Organization> Organizations { get; set; }
    }

    public class AdminListModel
    {
        public IList<User> Admins { get; set; }
        public IList<User> DepartmentalAdmins { get; set; }
    }
}
