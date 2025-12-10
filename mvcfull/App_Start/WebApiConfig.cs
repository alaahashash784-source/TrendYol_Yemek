// إعدادات Web API - لمسارات api/{controller}/{id}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace mvc_full
{
    public static class WebApiConfig
    {
        // تسجيل إعدادات Web API
        public static void Register(HttpConfiguration config)
        {
            // تفعيل التوجيه بالسمات (Attribute Routing)
            config.MapHttpAttributeRoutes();

            // تعريف المسار الافتراضي لـ API
            // النمط: api/{controller}/{id}
            // مثال: GET /api/products/1
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }  // id اختياري
            );
        }
    }
}
