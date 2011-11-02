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
        ConditionalApproval ConditionalApprovalAccess(ConditionalApprovalController controller, int id, out ActionResult redirectToAction, bool extraFetch = false);
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
        /// Checks:
        ///     ConditionalApproval exists
        ///     ConditionalApproval can be accessed for Current User using the Current User's Organizations or Workgroups
        /// Assumes:
        ///     User is in the DA role.
        /// </summary>
        /// <param name="controller">ConditionalApprovalController</param>
        /// <param name="id">ConditionalApproval Id</param>
        /// <param name="redirectToAction">Where to redirect to if there is a problem</param>
        /// <param name="extraFetch">Fetch the ConditionalApproval's Organization and Workgroup</param>
        /// <returns>ConditionalApproval when success</returns>
        public ConditionalApproval ConditionalApprovalAccess(ConditionalApprovalController controller, int id, out ActionResult redirectToAction, bool extraFetch = false)
        {
            ConditionalApproval conditionalApproval = null;
            if (extraFetch)
            {
                conditionalApproval = _repository.OfType<ConditionalApproval>().Queryable.Where(a => a.Id == id).Fetch(x => x.Organization).Fetch(x => x.Workgroup).SingleOrDefault();
            }
            else
            {
                conditionalApproval = _repository.OfType<ConditionalApproval>().GetNullableById(id);    
            }
            
            if(conditionalApproval == null)
            {
                controller.ErrorMessage = "Conditional Approval not found";
                redirectToAction = controller.RedirectToAction(a => a.Index());
            }

            var user = _repository.OfType<User>()
                .Queryable.Where(x => x.Id == _userIdentity.Current).Fetch(x => x.Organizations).Single();

            if(conditionalApproval.Workgroup != null)
            {
                var workgroupIds = GetWorkgroups(user).Select(x => x.Id).ToList();
                if(!workgroupIds.Contains(conditionalApproval.Workgroup.Id))
                {
                    controller.ErrorMessage = "No access to that workgroup";
                    {
                        redirectToAction = controller.RedirectToAction<ErrorController>(a => a.Index());
                        return null;
                    }
                }
            }
            else
            {
                var orgIds = user.Organizations.Select(x => x.Id).ToList();
                if(!orgIds.Contains(conditionalApproval.Organization.Id))
                {
                    controller.ErrorMessage = "No access to that organization";
                    {
                        redirectToAction = controller.RedirectToAction<ErrorController>(a => a.Index());
                        return null;
                    }
                }
            }

            redirectToAction = null;
            return conditionalApproval;
        }


        private IQueryable<Workgroup> GetWorkgroups(User user)
        {
            var orgIds = user.Organizations.Select(x => x.Id).ToArray();

            return _repository.OfType<Workgroup>().Queryable.Where(x => x.Organizations.Any(a => orgIds.Contains(a.Id)));

        }
    }
}