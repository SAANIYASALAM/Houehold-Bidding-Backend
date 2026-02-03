using Household_Bidding.Models.Enums;
using TaskStatus = Household_Bidding.Models.Enums.TaskStatus;

namespace Household_Bidding.DTOs.Worker;

public class NearbyTaskDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhoneNumber { get; set; } = string.Empty;
    public int ServiceCategoryId { get; set; }
    public string ServiceCategoryName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal Budget { get; set; }
    public DateTime ScheduledDate { get; set; }
    public double DistanceKm { get; set; }
    public int BidsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBidRequest
{
    public int TaskId { get; set; }
    public decimal ProposedAmount { get; set; }
    public int EstimatedHours { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class WorkerBidDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal ProposedAmount { get; set; }
    public int EstimatedHours { get; set; }
    public string Message { get; set; } = string.Empty;
    public BidStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WorkerTaskAssignmentDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhoneNumber { get; set; } = string.Empty;
    public TaskStatus TaskStatus { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal BidAmount { get; set; }
}
