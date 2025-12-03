// إعدادات الفلاتر - كود ينفذ قبل/بعد كل طلب (مثل معالجة الأخطاء)
using System.Web;
using System.Web.Mvc;

namespace mvc_full
{
    public class FilterConfig
    {
        // تسجيل الفلاتر العامة (تطبق على كل الكنترولرز)
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // فلتر معالجة الأخطاء - يعرض صفحة خطأ عند حدوث استثناء
            filters.Add(new HandleErrorAttribute());
            
            // يمكن إضافة فلاتر أخرى هنا مثل:
            // filters.Add(new AuthorizeAttribute()); // لطلب تسجيل دخول لكل الصفحات
        }
    }
}
