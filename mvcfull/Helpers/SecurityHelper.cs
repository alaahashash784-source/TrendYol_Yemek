
// ملف: SecurityHelper.cs
// الغرض: تشفير كلمات المرور والتحقق منها باستخدام PBKDF2
// الشرح: هذا الملف يضمن أمان كلمات المرور - لا تُخزّن بشكل مقروء أبداً

using System;
using System.Security.Cryptography;
using System.Text;

namespace mvc_full.Helpers
{
    // كلاس الأمان - تشفير PBKDF2 لكلمات المرور
    public static class SecurityHelper
    {
        
        // ثوابت التشفير - تحدد قوة التشفير
        
        private const int SaltSize = 16;      // حجم الـ Salt (ملح التشفير) - 16 بايت
        private const int HashSize = 32;      // حجم الـ Hash الناتج - 32 بايت
        private const int Iterations = 10000; // عدد التكرارات - كلما زاد أصبح أصعب للاختراق

       
        // تشفير كلمة المرور - تُستخدم عند التسجيل
        // المدخلات: كلمة المرور النصية
        // المخرجات: كلمة المرور المشفرة (Hash + Salt)
       
        public static string HashPassword(string password)
        {
            // التحقق من وجود كلمة مرور
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            // إنشاء Salt عشوائي - يختلف لكل مستخدم
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt); // توليد بايتات عشوائية آمنة
            }

            // إنشاء الـ Hash باستخدام PBKDF2
            byte[] hash = CreateHash(password, salt);

            // دمج الـ Salt مع الـ Hash في مصفوفة واحدة
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);        // نسخ Salt أولاً
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize); // ثم نسخ Hash

            // تحويل لـ Base64 للتخزين في قاعدة البيانات
            return Convert.ToBase64String(hashBytes);
        }

        
        // التحقق من كلمة المرور - تُستخدم عند تسجيل الدخول
        // المدخلات: كلمة المرور المُدخلة + الـ Hash المخزن
        // المخرجات: true إذا تطابقت، false إذا لم تتطابق
        
        public static bool VerifyPassword(string password, string storedHash)
        {
            // التحقق من وجود القيم
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            try
            {
                // تحويل الـ Hash المخزن من Base64 إلى بايتات
                byte[] hashBytes = Convert.FromBase64String(storedHash);

                // التحقق من صحة طول الـ Hash
                if (hashBytes.Length != SaltSize + HashSize)
                {
                    // للتوافق مع كلمات المرور القديمة (غير مشفرة)
                    return storedHash == password || 
                           storedHash == Convert.ToBase64String(Encoding.UTF8.GetBytes(password + "salt"));
                }

                // استخراج الـ Salt من البداية
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // استخراج الـ Hash المخزن
                byte[] storedPasswordHash = new byte[HashSize];
                Array.Copy(hashBytes, SaltSize, storedPasswordHash, 0, HashSize);

                // حساب Hash لكلمة المرور المُدخلة باستخدام نفس الـ Salt
                byte[] computedHash = CreateHash(password, salt);

                // مقارنة الـ Hash المحسوب مع المخزن
                return SlowEquals(storedPasswordHash, computedHash);
            }
            catch
            {
                // في حالة الخطأ، جرب المقارنة المباشرة (للتوافق)
                return storedHash == password;
            }
        }

        
        // إنشاء Hash باستخدام خوارزمية PBKDF2
        
        private static byte[] CreateHash(string password, byte[] salt)
        {
            // PBKDF2 = Password-Based Key Derivation Function 2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        
        // مقارنة آمنة - تمنع هجمات التوقيت (Timing Attacks)
       
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            // مقارنة كل البايتات بنفس الوقت (ثابت الزمن)
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i]; // XOR - إذا تطابقا = 0
            }
            return diff == 0;
        }

        
        // إنشاء رمز أمان عشوائي (Token)
       
        public static string GenerateSecureToken(int length = 32)
        {
            byte[] tokenBytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes);
        }

       
        // تنظيف المدخلات - منع هجمات XSS
        
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // استبدال الرموز الخطرة بنسخ آمنة
            return input
                .Replace("<", "&lt;")     // منع HTML tags
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")  // منع إغلاق الـ attributes
                .Replace("'", "&#39;");
        }
    }
}
