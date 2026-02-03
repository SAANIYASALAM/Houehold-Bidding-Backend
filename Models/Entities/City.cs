namespace Household_Bidding.Models.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<CustomerProfile> CustomerProfiles { get; set; } = new List<CustomerProfile>();
    public ICollection<WorkerProfile> WorkerProfiles { get; set; } = new List<WorkerProfile>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}
