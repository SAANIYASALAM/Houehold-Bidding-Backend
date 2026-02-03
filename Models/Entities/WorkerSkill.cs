namespace Household_Bidding.Models.Entities;

public class WorkerSkill
{
    public int Id { get; set; }
    public int WorkerProfileId { get; set; }
    public int ServiceCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public WorkerProfile WorkerProfile { get; set; } = null!;
    public ServiceCategory ServiceCategory { get; set; } = null!;
}
