using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Attributes;
using Purchasing.Mvc.Helpers;
using UCDArch.Data.NHibernate;
using UCDArch.Web.Controller;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Mvc.Controllers
{
    [Version(MajorVersion = 2)]
    [Profile]
    [Authorize]
    public abstract class ApplicationController : SuperController
    {
        [CloseNHibernateSession]
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
    }
}
