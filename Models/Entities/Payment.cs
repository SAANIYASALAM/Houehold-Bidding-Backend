using Household_Bidding.Models.Enums;

namespace Household_Bidding.Models.Entities;

public class Payment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int CustomerProfileId { get; set; }
    public int WorkerProfileId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public CustomerProfile CustomerProfile { get; set; } = null!;
    public WorkerProfile WorkerProfile { get; set; } = null!;
}
