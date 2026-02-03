namespace Household_Bidding.Models.Entities;

public class TaskAssignment
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int WorkerProfileId { get; set; }
    public int BidId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public WorkerProfile WorkerProfile { get; set; } = null!;
    public Bid Bid { get; set; } = null!;
}
