// إعدادات التوجيه - يحدد كيف يترجم URL إلى Controller/Action
// النمط: {controller}/{action}/{id} مثل: /Food/Details/5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace mvc_full
{
    public class RouteConfig
    {
        // تسجيل مسارات URL للتطبيق
        public static void RegisterRoutes(RouteCollection routes)
        {
            // تجاهل طلبات الموارد الخاصة (.axd)
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // المسار الافتراضي - الأهم!
            // {controller} = اسم الكنترولر (مثل: Home, Food, Account)
            // {action} = اسم الدالة (مثل: Index, Details, Create)
            // {id} = معرف اختياري
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { 
                    controller = "Home",   // الكنترولر الافتراضي
                    action = "Index",      // الدالة الافتراضية
                    id = UrlParameter.Optional  // id اختياري
                }
            );
        }
    }
}
