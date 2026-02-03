namespace Household_Bidding.Models.Entities;

public class WorkerProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Address { get; set; } = string.Empty;
    public int CityId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal HourlyRate { get; set; }
    public int ExperienceYears { get; set; }
    public string Bio { get; set; } = string.Empty;
    public double AverageRating { get; set; } = 0.0;
    public int TotalReviews { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public City City { get; set; } = null!;
    public ICollection<WorkerSkill> WorkerSkills { get; set; } = new List<WorkerSkill>();
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
    public ICollection<Review> ReceivedReviews { get; set; } = new List<Review>();
}
