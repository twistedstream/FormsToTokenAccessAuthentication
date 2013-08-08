using System;
using System.Web.Mvc;

namespace TS.FormsToTokenAccessAuthentication.Sample.Service.Filters
{
    // This attribute needs to be used with ASP.NET MVC controllers that require access to the membership DB
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class MvcInitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SimpleMembershipInitializer.Initialize();
        }
    }
}
