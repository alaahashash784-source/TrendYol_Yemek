
// ملف: CustomAuthorizationFilter.cs
// الغرض: فلتر للتحقق من تسجيل الدخول (للمستخدمين العاديين)
// الشرح: يمنع أي زائر غير مسجل من الوصول للصفحات المحمية
// الاستخدام: [CustomAuthorizationFilter] فوق الـ Controller أو Action

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace mvc_full.Filters
{
    
    // فلتر التحقق من تسجيل الدخول (للمستخدمين العاديين)
    // الفرق عن AdminAuthorizationFilter: هذا لا يتحقق من IsAdmin
    
    public class CustomAuthorizationFilter : ActionFilterAttribute
    {
        
        // تُنفذ قبل أي Action محمي بهذا الفلتر
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            // التحقق: هل المستخدم مسجل دخول؟
            // Session["MusteriId"] == null تعني أنه زائر (غير مسجل)
            
            if (HttpContext.Current.Session["MusteriId"] == null)
            {
                // توجيه لصفحة تسجيل الدخول مع حفظ الرابط الأصلي
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },     // كنترولر الحسابات
                        { "action", "Login" },           // صفحة تسجيل الدخول
                        { "returnUrl", filterContext.HttpContext.Request.RawUrl } // للعودة بعد تسجيل الدخول
                    });
            }

            // إذا مسجل دخول ← استمر بتنفيذ الـ Action
            base.OnActionExecuting(filterContext);
        }
    }
}
