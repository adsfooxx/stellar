using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class EcPay
{
    /// <summary>
    /// 流水號
    /// </summary>
    public int EcPayId { get; set; }

    /// <summary>
    /// 訂單ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 特店編號
    /// </summary>
    public string? MerchantId { get; set; }

    /// <summary>
    /// 特電交易編號
    /// </summary>
    public string? MerchantTradeNo { get; set; }

    /// <summary>
    /// 特電旗下店鋪代號
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 交易狀態
    /// </summary>
    public int? RtnCode { get; set; }

    /// <summary>
    /// 交易訊息
    /// </summary>
    public string? RtnMsg { get; set; }

    /// <summary>
    /// 綠界的交易編號
    /// </summary>
    public string? TradeNo { get; set; }

    /// <summary>
    /// 交易金額
    /// </summary>
    public int? TradeAmt { get; set; }

    /// <summary>
    /// 付款時間
    /// </summary>
    public string? PaymentDate { get; set; }

    /// <summary>
    /// 特電選擇的付款方式
    /// </summary>
    public string? PaymentType { get; set; }

    /// <summary>
    /// 交易手續費金額
    /// </summary>
    public int? PaymentTypeChargeFee { get; set; }

    /// <summary>
    /// 訂單成立時間
    /// </summary>
    public string? TradeDate { get; set; }

    /// <summary>
    /// 是否為模擬付款
    /// </summary>
    public int? SimulatePaid { get; set; }

    /// <summary>
    /// 檢查碼
    /// </summary>
    public string? CheckMacValue { get; set; }

    public virtual Order Order { get; set; } = null!;
}
