using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Purchasing.Core;
using UCDArch.Web.Controller;

namespace Purchasing.Web.Controllers
{
    public class ListSyncController : SuperController
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ListSyncController(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public ActionResult Users()
        {
            var users =
                _repositoryFactory.UserRepository.Queryable.Select(
                    x => new UserEmailResult {Email = x.Email, Name = x.FirstName + " " + x.LastName});

            return Content(WriteUsers(users), "text/plain");
        }

        public ActionResult Admins()
        {
            var adminUsers = from user in _repositoryFactory.UserRepository.Queryable
                             where user.Roles.Any()
                             select new UserEmailResult { Email = user.Email, Name = user.FirstName + " " + user.LastName };

            return Content(WriteUsers(adminUsers), "text/plain");
        }

        private string WriteUsers(IEnumerable<UserEmailResult> users)
        {
            var str = new StringBuilder();

            foreach (var user in users)
            {
                str.AppendFormat("{0} {1}{2}", user.Email, user.Name, Environment.NewLine);
            }

            return str.ToString();
        }

        private class UserEmailResult
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }
    }
}