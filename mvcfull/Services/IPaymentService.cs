// ═══════════════════════════════════════════════════════════════════════════
// Payment Strategy Pattern - نمط الاستراتيجية لطرق الدفع
// ═══════════════════════════════════════════════════════════════════════════
// 
// لماذا نستخدم Interface؟
// ─────────────────────────────────────────────────────────────────────────────
// 1. المرونة: يمكن إضافة طريقة دفع جديدة بسهولة (فقط أنشئ class جديد)
// 2. SOLID Principles: 
//    - Single Responsibility: كل class مسؤول عن طريقة دفع واحدة
//    - Open/Closed: مفتوح للتوسع، مغلق للتعديل
//    - Dependency Inversion: نعتمد على Interface لا على Implementation
// 3. سهولة الاختبار: يمكن عمل Mock للـ Interface
//
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Linq;
using mvc_full.Models;

namespace mvc_full.Services
{
    /// <summary>
    /// واجهة طريقة الدفع - كل طريقة دفع تنفذ هذه الواجهة
    /// Payment Method Interface - Every payment method implements this
    /// </summary>
    public interface IPaymentMethod
    {
        /// <summary>
        /// اسم طريقة الدفع بالتركية
        /// </summary>
        string Name { get; }

        /// <summary>
        /// معرف فريد لطريقة الدفع
        /// </summary>
        string Code { get; }

        /// <summary>
        /// هل تتطلب هذه الطريقة معالجة فورية؟
        /// </summary>
        bool RequiresOnlineProcessing { get; }

        /// <summary>
        /// معالجة الدفع
        /// </summary>
        /// <param name="order">الطلب</param>
        /// <param name="amount">المبلغ</param>
        /// <returns>نتيجة العملية</returns>
        PaymentResult ProcessPayment(OrderPage order, decimal amount);

        /// <summary>
        /// التحقق من صحة بيانات الدفع
        /// </summary>
        bool ValidatePaymentDetails(string details);
    }

    /// <summary>
    /// نتيجة عملية الدفع
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }

        public static PaymentResult Successful(string message = "Ödeme başarılı")
        {
            return new PaymentResult
            {
                Success = true,
                Message = message,
                TransactionId = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()
            };
        }

        public static PaymentResult Failed(string message)
        {
            return new PaymentResult
            {
                Success = false,
                Message = message
            };
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // تنفيذات طرق الدفع - Payment Method Implementations
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// الدفع نقداً عند الاستلام
    /// </summary>
    public class CashOnDeliveryPayment : IPaymentMethod
    {
        public string Name => "Kapıda Nakit Ödeme";
        public string Code => "CASH";
        public bool RequiresOnlineProcessing => false;

        public PaymentResult ProcessPayment(OrderPage order, decimal amount)
        {
            // لا حاجة لمعالجة - الدفع عند التسليم
            return PaymentResult.Successful("Ödeme teslimat sırasında nakit olarak alınacaktır.");
        }

        public bool ValidatePaymentDetails(string details)
        {
            return true; // لا حاجة للتحقق
        }
    }

    /// <summary>
    /// الدفع بالبطاقة عند الاستلام
    /// </summary>
    public class CardOnDeliveryPayment : IPaymentMethod
    {
        public string Name => "Kapıda Kredi Kartı";
        public string Code => "CARD_DELIVERY";
        public bool RequiresOnlineProcessing => false;

        public PaymentResult ProcessPayment(OrderPage order, decimal amount)
        {
            return PaymentResult.Successful("Ödeme teslimat sırasında kredi kartı ile alınacaktır.");
        }

        public bool ValidatePaymentDetails(string details)
        {
            return true;
        }
    }

    /// <summary>
    /// الدفع الإلكتروني
    /// </summary>
    public class OnlinePayment : IPaymentMethod
    {
        public string Name => "Online Ödeme";
        public string Code => "ONLINE";
        public bool RequiresOnlineProcessing => true;

        public PaymentResult ProcessPayment(OrderPage order, decimal amount)
        {
            // هنا يمكن ربط بوابة دفع حقيقية مثل iyzico, PayTR
            // محاكاة عملية الدفع
            return PaymentResult.Successful("Online ödeme başarıyla işlendi.");
        }

        public bool ValidatePaymentDetails(string details)
        {
            // التحقق من صحة بيانات البطاقة
            return !string.IsNullOrEmpty(details);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // إضافة طريقة دفع جديدة - مثال: PayPal
    // ═══════════════════════════════════════════════════════════════════════════
    // 
    // لإضافة PayPal، فقط أنشئ class جديد:
    //
    // public class PayPalPayment : IPaymentMethod
    // {
    //     public string Name => "PayPal ile Öde";
    //     public string Code => "PAYPAL";
    //     public bool RequiresOnlineProcessing => true;
    //     
    //     public PaymentResult ProcessPayment(OrderPage order, decimal amount)
    //     {
    //         // اتصل بـ PayPal API هنا
    //         return PaymentResult.Successful("PayPal ödemesi başarılı.");
    //     }
    //     
    //     public bool ValidatePaymentDetails(string details) => true;
    // }
    //
    // ثم أضفها للـ Factory أدناه!
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// مصنع طرق الدفع - يُرجع طريقة الدفع المناسبة
    /// Factory Pattern for Payment Methods
    /// </summary>
    public class PaymentMethodFactory
    {
        private static readonly Dictionary<string, IPaymentMethod> _methods = 
            new Dictionary<string, IPaymentMethod>
            {
                { "CASH", new CashOnDeliveryPayment() },
                { "CARD_DELIVERY", new CardOnDeliveryPayment() },
                { "ONLINE", new OnlinePayment() },
                // إضافة طريقة جديدة؟ فقط أضف سطراً هنا!
                // { "PAYPAL", new PayPalPayment() },
            };

        /// <summary>
        /// الحصول على طريقة الدفع بالكود
        /// </summary>
        public static IPaymentMethod GetPaymentMethod(string code)
        {
            if (_methods.TryGetValue(code.ToUpper(), out var method))
                return method;

            // Default: نقداً عند الاستلام
            return new CashOnDeliveryPayment();
        }

        /// <summary>
        /// الحصول على جميع طرق الدفع المتاحة
        /// </summary>
        public static IEnumerable<IPaymentMethod> GetAllPaymentMethods()
        {
            return _methods.Values;
        }

        /// <summary>
        /// إضافة طريقة دفع جديدة (Runtime)
        /// </summary>
        public static void RegisterPaymentMethod(string code, IPaymentMethod method)
        {
            _methods[code.ToUpper()] = method;
        }
    }
}
