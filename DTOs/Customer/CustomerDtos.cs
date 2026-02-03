using Household_Bidding.Models.Enums;
using TaskStatus = Household_Bidding.Models.Enums.TaskStatus;

namespace Household_Bidding.DTOs.Customer;

public class CreateTaskRequest
{
    public int ServiceCategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal Budget { get; set; }
    public DateTime ScheduledDate { get; set; }
}

public class UpdateTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public decimal? Budget { get; set; }
    public DateTime? ScheduledDate { get; set; }
}

public class TaskDto
{
    public int Id { get; set; }
    public int CustomerProfileId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int ServiceCategoryId { get; set; }
    public string ServiceCategoryName { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal Budget { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TaskStatus Status { get; set; }
    public int BidsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class BidDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int WorkerProfileId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public string WorkerPhoneNumber { get; set; } = string.Empty;
    public double WorkerRating { get; set; }
    public int WorkerTotalReviews { get; set; }
    public int WorkerExperienceYears { get; set; }
    public decimal ProposedAmount { get; set; }
    public int EstimatedHours { get; set; }
    public string Message { get; set; } = string.Empty;
    public BidStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AssignWorkerRequest
{
    public int BidId { get; set; }
}

public class TaskRevisionRequest
{
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class TaskRevisionDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public string? Resolution { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class PaymentRequest
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
}

public class PaymentDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReviewRequest
{
    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
}

public class ReviewDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int WorkerProfileId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
