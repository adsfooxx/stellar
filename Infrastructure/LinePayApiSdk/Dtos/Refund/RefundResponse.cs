using ApplicationCore.LinePay.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationCore.LinePayApiSdk.Dtos.Refund
{
    public class RefundResponse : LinePayApiResponseBase
    {
        /// <summary>
        /// 交易資訊。
        /// </summary>
        [JsonPropertyName("info")]
        public RefundTransactionInfo Info { get; set; }
    }

    /// <summary>
    /// 表示退款交易資訊。
    /// </summary>
    public class RefundTransactionInfo
    {
        /// <summary>
        /// 退款序號（該次退款產生的新序號, 19 digits）。
        /// </summary>
        [JsonPropertyName("refundTransactionId")]
        public long RefundTransactionId { get; set; }

        /// <summary>
        /// 退款日期（ISO 8601）。
        /// </summary>
        [JsonPropertyName("refundTransactionDate")]
        public string RefundTransactionDate { get; set; }
    }
}
