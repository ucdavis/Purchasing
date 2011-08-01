using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

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
            var admins = _userRepository.Queryable;
            return View(admins.ToList());
        }
    }
}
