using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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

        public AdminController(IRepositoryWithTypedId<User,string> userRepository, IRepositoryWithTypedId<Role,string> roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            var admins = _roleRepository.Queryable.Where(x => x.Name.EndsWith("Admin")).Fetch(x => x.Users).ToList();

            var model = new AdminListModel()
                            {
                                Admins = admins.Single(x => x.Name == "Admin").Users,
                                DepartmentalAdmins = admins.Single(x => x.Name == "DepartmentalAdmin").Users
                            };

            return View(model);
        }
    }

    public class AdminListModel
    {
        public IList<User> Admins { get; set; }
        public IList<User> DepartmentalAdmins { get; set; }
    }
}
