
// ملف: AdminAuthorizationFilter.cs
// الغرض: فلتر للتحقق من صلاحيات الأدمن
// الشرح: يمنع أي مستخدم غير أدمن من الوصول للصفحات المحمية
// الاستخدام: [AdminAuthorizationFilter] فوق الـ Controller أو Action

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace mvc_full.Filters
{
   
    // فلتر صلاحيات الأدمن
    // يرث من ActionFilterAttribute ليتم تنفيذه قبل أي Action
    
    public class AdminAuthorizationFilter : ActionFilterAttribute
    {
        
        // هذه الدالة تُنفذ تلقائياً قبل أي Action محمي
       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // جلب الـ Session الحالية
            var session = HttpContext.Current.Session;

            
            // الخطوة 1: التحقق من تسجيل الدخول
            // إذا لم يكن مسجل دخول ← توجيه لصفحة Login
            
            if (session["MusteriId"] == null)
            {
                // إنشاء توجيه لصفحة تسجيل الدخول
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },     // الكنترولر
                        { "action", "Login" },           // الأكشن
                        { "returnUrl", filterContext.HttpContext.Request.RawUrl } // الرابط الأصلي للعودة بعد تسجيل الدخول
                    });
                return; // إيقاف التنفيذ
            }

            
            // الخطوة 2: التحقق من صلاحية الأدمن
            // مسجل دخول لكن هل هو أدمن؟
            
            bool isAdmin = session["IsAdmin"] != null && (bool)session["IsAdmin"];
            
            if (!isAdmin)
            {
                // مستخدم عادي يحاول دخول صفحة أدمن ← خطأ 403 (ممنوع)
                filterContext.Result = new HttpStatusCodeResult(403, "Bu islem icin admin yetkisi gereklidir.");
                return;
            }

            
            // الخطوة 3: مرور ناجح ← استمر بتنفيذ الـ Action
            
            base.OnActionExecuting(filterContext);
        }
    }
}
