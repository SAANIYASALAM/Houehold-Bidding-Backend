namespace Household_Bidding.Models.Entities;

public class ServiceCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<WorkerSkill> WorkerSkills { get; set; } = new List<WorkerSkill>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
