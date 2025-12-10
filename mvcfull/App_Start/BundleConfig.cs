// إعدادات الحزم - تجميع وضغط ملفات CSS و JS لتحسين الأداء
using System.Web;
using System.Web.Optimization;

namespace mvc_full
{
    public class BundleConfig
    {
        // تسجيل حزم الملفات
        public static void RegisterBundles(BundleCollection bundles)
        {
            // حزمة jQuery - مكتبة جافاسكريبت الأساسية
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // حزمة التحقق - للتحقق من صحة النماذج (Forms)
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // حزمة Modernizr - لكشف ميزات المتصفح
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // حزمة Bootstrap - للمكونات التفاعلية (قوائم، نوافذ...)
            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            // حزمة CSS - جميع ملفات التنسيق
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",    // Bootstrap الأساسي
                      "~/Content/site.css",         // الموقع العام
                      "~/Content/custom-styles.css")); // التنسيقات المخصصة

            // تعطيل الضغط للتطوير (لرؤية الملفات كاملة)
            // في الإنتاج: غيرها لـ true لتحسين الأداء
            BundleTable.EnableOptimizations = false;
        }
    }
}
