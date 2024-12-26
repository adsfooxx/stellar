using Web.Enums;

namespace Web.Extensions
{
    public static class PaymentTypeExtensions
    {
        public static string GetPaymentTypeName(this int paymentTypeId)
        {
            return paymentTypeId switch
            {
                1 => PaymentType.綠界.ToString(),
                2 => PaymentType.LinePay.ToString(),
                _ => "信用卡"  
            };
        }
    }
}
