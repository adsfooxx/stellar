using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities;

public partial class LinePay
{
    public int Id { get; set; }

    public string OrderId { get; set; } = null!;

    public string TransactionId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string? Status { get; set; }

    public string? ConfirmUrl { get; set; }

    public string? CancelUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
