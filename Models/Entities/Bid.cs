using Household_Bidding.Models.Enums;

namespace Household_Bidding.Models.Entities;

public class Bid
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int WorkerProfileId { get; set; }
    public decimal ProposedAmount { get; set; }
    public int EstimatedHours { get; set; }
    public string Message { get; set; } = string.Empty;
    public BidStatus Status { get; set; } = BidStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public WorkerProfile WorkerProfile { get; set; } = null!;
}
