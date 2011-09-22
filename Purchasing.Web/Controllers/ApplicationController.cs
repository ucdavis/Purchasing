using System.Web.Mvc;
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
        public string ErrorMessage
        {
            get { return TempData[TEMP_DATA_ERROR_MESSAGE_KEY] as string; }
            set { TempData[TEMP_DATA_ERROR_MESSAGE_KEY] = value; }
        }

        private const string TEMP_DATA_ERROR_MESSAGE_KEY = "ErrorMessage";
    }
}
