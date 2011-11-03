using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Controllers;
using UCDArch.Core.PersistanceSupport;
using MvcContrib;
using System.Linq;
using System.Linq.Expressions;

namespace Purchasing.Web.Services
{
    public interface ISecurityService
    {
        bool HasWorkgroupOrOrganizationAccess(Workgroup workgroup, Organization organization, out string message);
    }

    public class SecurityService :  ISecurityService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRepository _repository;    

        public SecurityService(IRepository repository, IUserIdentity userIdentity)
        {
            _userIdentity = userIdentity;
            _repository = repository;
        }

        /// <summary>
        /// Checks if the current user has access to the workgroup or the organization.
        /// </summary>
        /// <param name="workgroup"></param>
        /// <param name="organization"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool HasWorkgroupOrOrganizationAccess(Workgroup workgroup, Organization organization, out string message)
        {
            if(workgroup == null && organization == null)
            {
                message = "Workgroup and Organization not found.";
                return false;
            }

            var user = _repository.OfType<User>()
                .Queryable.Where(x => x.Id == _userIdentity.Current).Fetch(x => x.Organizations).Single();

            if(workgroup != null)
            {
                var workgroupIds = GetWorkgroups(user).Select(x => x.Id).ToList();
                if(!workgroupIds.Contains(workgroup.Id))
                {
                    message = "No access to that workgroup";
                    return false;
                }
            }
            else
            {
                var orgIds = user.Organizations.Select(x => x.Id).ToList();
                if(!orgIds.Contains(organization.Id))
                {
                    message = "No access to that organization";
                    return false;
                }
            }

            message = null;
            return true;
        }


        private IQueryable<Workgroup> GetWorkgroups(User user)
        {
            var orgIds = user.Organizations.Select(x => x.Id).ToArray();

            return _repository.OfType<Workgroup>().Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

        }
    }
}