using System;

namespace TS.FormsToTokenAccessAuthentication.Sample.Service.Filters
{
    // This attribute needs to be used with ASP.NET Web API controllers that require access to the membership DB
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class WebApiInitializeSimpleMembershipAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            SimpleMembershipInitializer.Initialize();
        }
    }
}