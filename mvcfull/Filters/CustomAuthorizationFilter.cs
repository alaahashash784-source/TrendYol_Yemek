using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace mvc_full.Filters
{
    public class CustomAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["MusteriId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" },
                        { "returnUrl", filterContext.HttpContext.Request.RawUrl }
                    });
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
