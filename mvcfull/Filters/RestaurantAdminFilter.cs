// ═══════════════════════════════════════════════════════════════════════════════
// ملف: RestaurantAdminFilter.cs
// الغرض: فلتر للتحقق من صلاحيات مدير المطعم
// الشرح: يسمح لمدير المطعم بإدارة وجبات مطعمه فقط
// الاستخدام: [RestaurantAdminFilter] فوق الـ Action
// ═══════════════════════════════════════════════════════════════════════════════

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using mvc_full.Models;
using System.Linq;

namespace mvc_full.Filters
{
    /// <summary>
    /// فلتر صلاحيات مدير المطعم
    /// يسمح للأدمن الرئيسي أو مدير المطعم المعني
    /// </summary>
    public class RestaurantAdminFilter : ActionFilterAttribute
    {
        private ABCDbContext db = new ABCDbContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = HttpContext.Current.Session;

            // الخطوة 1: التحقق من تسجيل الدخول
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

            // الخطوة 2: إذا كان أدمن رئيسي - يسمح له بكل شيء
            bool isAdmin = session["IsAdmin"] != null && (bool)session["IsAdmin"];
            if (isAdmin)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // الخطوة 3: التحقق من أنه مدير مطعم
            bool isRestoranAdmin = session["IsRestoranAdmin"] != null && (bool)session["IsRestoranAdmin"];
            if (!isRestoranAdmin)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "AccessDenied" }
                    });
                return;
            }

            // الخطوة 4: التحقق من أن العملية على مطعمه
            int? userRestoranId = session["RestoranId"] as int?;
            if (!userRestoranId.HasValue)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "AccessDenied" }
                    });
                return;
            }

            // جلب RestoranId من المعاملات
            int? requestRestoranId = null;

            // محاولة جلب من Route
            if (filterContext.RouteData.Values["restoranId"] != null)
            {
                requestRestoranId = Convert.ToInt32(filterContext.RouteData.Values["restoranId"]);
            }
            // أو من Form
            else if (filterContext.HttpContext.Request.Form["RestoranId"] != null)
            {
                requestRestoranId = Convert.ToInt32(filterContext.HttpContext.Request.Form["RestoranId"]);
            }
            // أو من YemekId لمعرفة المطعم
            else if (filterContext.RouteData.Values["id"] != null)
            {
                int yemekId = Convert.ToInt32(filterContext.RouteData.Values["id"]);
                var yemek = db.Yemekler.Find(yemekId);
                if (yemek != null)
                {
                    requestRestoranId = yemek.RestoranId;
                }
            }

            // التحقق من التطابق
            if (requestRestoranId.HasValue && requestRestoranId.Value != userRestoranId.Value)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "AccessDenied" }
                    });
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }

    /// <summary>
    /// فلتر يسمح للأدمن الرئيسي أو مدير أي مطعم
    /// </summary>
    public class AnyRestaurantAdminFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = HttpContext.Current.Session;

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

            bool isAdmin = session["IsAdmin"] != null && (bool)session["IsAdmin"];
            bool isRestoranAdmin = session["IsRestoranAdmin"] != null && (bool)session["IsRestoranAdmin"];

            if (!isAdmin && !isRestoranAdmin)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "AccessDenied" }
                    });
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
