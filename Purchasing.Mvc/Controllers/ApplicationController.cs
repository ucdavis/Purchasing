using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Attributes;
using Purchasing.Web.Helpers;
using UCDArch.Data.NHibernate;
using UCDArch.Web.Controller;

namespace Purchasing.Mvc.Controllers
{
    [Version(MajorVersion = 1)]
    [Profile]
    [Authorize]
    public abstract class ApplicationController : SuperController
    {
        public User GetCurrentUser()
        {
            return Repository.OfType<User>().Queryable.Single(x => x.Id == CurrentUser.Identity.Name);
        }

        public string ErrorMessage
        {
            get { return TempData[TEMP_DATA_ERROR_MESSAGE_KEY] as string; }
            set { TempData[TEMP_DATA_ERROR_MESSAGE_KEY] = value; }
        }

        private const string TEMP_DATA_ERROR_MESSAGE_KEY = "ErrorMessage";

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            NHibernateSessionManager.Instance.CloseSession();
            base.OnResultExecuted(filterContext);
        }
    }
}
