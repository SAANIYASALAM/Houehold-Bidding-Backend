namespace Household_Bidding.Models.Entities;

public class TaskRevision
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsResolved { get; set; } = false;
    public string? Resolution { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Task Task { get; set; } = null!;
}
