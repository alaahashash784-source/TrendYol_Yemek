
// الغرض: إدارة جلسات المستخدمين (Session Management)
// الشرح: هذا الملف يتحكم بتخزين واسترجاع بيانات المستخدم أثناء تصفحه للموقع

using System;
using System.Web;

namespace mvc_full.Helpers
{
    // كلاس ثابت (static) - لا يحتاج إنشاء كائن منه
    public static class SessionHelper
    {
        
        // التحقق: هل المستخدم مسجل دخول؟
        // الآلية: نبحث عن MusteriId في الـ Session، إذا وجدناه = مسجل دخول
        
        public static bool IsUserLoggedIn()
        {
            // Session["MusteriId"] != null تعني أن المستخدم سجل دخوله
            return HttpContext.Current.Session["MusteriId"] != null;
        }

        
        // جلب رقم المستخدم الحالي (ID)
        // الإرجاع: رقم المستخدم أو null إذا لم يكن مسجل دخول
       
        public static int? GetCurrentUserId()
        {
            // التحقق من وجود الـ Session أولاً
            if (HttpContext.Current.Session["MusteriId"] != null)
            {
                // تحويل القيمة من object إلى int
                return (int)HttpContext.Current.Session["MusteriId"];
            }
            // إذا لم يكن مسجل = نرجع null
            return null;
        }

        
        // جلب اسم المستخدم الحالي
        // الإرجاع: اسم المستخدم أو "Misafir" (ضيف) إذا لم يكن مسجل
       
        public static string GetCurrentUserName()
        {
            if (HttpContext.Current.Session["MusteriAd"] != null)
            {
                return HttpContext.Current.Session["MusteriAd"].ToString();
            }
            // الاسم الافتراضي للزوار
            return "Misafir";
        }

        
        // حفظ بيانات المستخدم في الـ Session (بعد تسجيل الدخول)
        // المدخلات: رقم المستخدم واسمه
        
        public static void SetUserSession(int musteriId, string musteriAd)
        {
            // حفظ رقم المستخدم - يستخدم للتحقق من تسجيل الدخول
            HttpContext.Current.Session["MusteriId"] = musteriId;
            // حفظ اسم المستخدم - يعرض في الـ Navbar
            HttpContext.Current.Session["MusteriAd"] = musteriAd;
        }

        
        // تسجيل الخروج - مسح جميع بيانات الـ Session
        
        public static void ClearUserSession()
        {
            // مسح جميع البيانات المخزنة
            HttpContext.Current.Session.Clear();
            // إنهاء الجلسة بالكامل
            HttpContext.Current.Session.Abandon();
        }
    }
}
