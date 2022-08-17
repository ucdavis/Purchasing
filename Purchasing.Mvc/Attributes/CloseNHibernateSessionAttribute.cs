using Serilog;
using Microsoft.AspNetCore.Mvc.Filters;
using UCDArch.Data.NHibernate;

namespace Purchasing.Mvc.Attributes
{
    public class CloseNHibernateSessionAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            NHibernateSessionManager.Instance.CloseSession();
        }
    }
}