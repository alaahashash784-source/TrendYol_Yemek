using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace mvc_full.Filters
{
    /// <summary>
    /// Authorization filter that ensures only admin users can access certain actions.
    /// Use this attribute on controller actions that should be restricted to admins only.
    /// </summary>
    public class AdminAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = HttpContext.Current.Session;

            // First check if user is logged in
            if (session["MusteriId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" },
                        { "returnUrl", filterContext.HttpContext.Request.RawUrl }
                    });
                return;
            }

            // Then check if user is admin
            bool isAdmin = session["IsAdmin"] != null && (bool)session["IsAdmin"];
            if (!isAdmin)
            {
                // User is logged in but not admin - return 403 Forbidden
                filterContext.Result = new HttpStatusCodeResult(403, "Bu islem icin admin yetkisi gereklidir.");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
