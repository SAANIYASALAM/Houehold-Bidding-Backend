using Household_Bidding.Models.Enums;
using TaskStatus = Household_Bidding.Models.Enums.TaskStatus;

namespace Household_Bidding.Models.Entities;

public class Task
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }
    public int ServiceCategoryId { get; set; }
    public int CityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal Budget { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Open;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public CustomerProfile CustomerProfile { get; set; } = null!;
    public ServiceCategory ServiceCategory { get; set; } = null!;
    public City City { get; set; } = null!;
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public TaskAssignment? TaskAssignment { get; set; }
    public ICollection<TaskRevision> TaskRevisions { get; set; } = new List<TaskRevision>();
    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
}
