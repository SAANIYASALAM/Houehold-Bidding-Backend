namespace Household_Bidding.Models.Entities;

public class Review
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int CustomerProfileId { get; set; }
    public int WorkerProfileId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
    public CustomerProfile CustomerProfile { get; set; } = null!;
    public WorkerProfile WorkerProfile { get; set; } = null!;
}
