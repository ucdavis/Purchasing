﻿using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web.Helpers;
using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;

namespace Purchasing.Web.Controllers
{
    [Version(MajorVersion = 1)]
    [Profile]
    [Authorize]
    public abstract class ApplicationController : SuperController
    {
        public User GetCurrentUser()
        {
            return Repository.OfType<User>().Queryable.Where(x => x.Id == CurrentUser.Identity.Name).Single();
        }

        public string ErrorMessage
        {
            get { return TempData[TEMP_DATA_ERROR_MESSAGE_KEY] as string; }
            set { TempData[TEMP_DATA_ERROR_MESSAGE_KEY] = value; }
        }

        private const string TEMP_DATA_ERROR_MESSAGE_KEY = "ErrorMessage";
    }
}
