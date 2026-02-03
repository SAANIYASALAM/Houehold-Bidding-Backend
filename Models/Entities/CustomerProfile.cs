namespace Household_Bidding.Models.Entities;

public class CustomerProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Address { get; set; } = string.Empty;
    public int CityId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public City City { get; set; } = null!;
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
