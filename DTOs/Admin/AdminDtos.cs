using Household_Bidding.Models.Enums;
using TaskStatus = Household_Bidding.Models.Enums.TaskStatus;

namespace Household_Bidding.DTOs.Admin;

public class UserManagementDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public bool IsSuspended { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TaskManagementDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ServiceCategoryName { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public decimal Budget { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? WorkerName { get; set; }
}

public class PaymentManagementDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string WorkerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReviewManagementDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string WorkerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SuspendUserRequest
{
    public bool Suspend { get; set; }
}

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalWorkers { get; set; }
    public int TotalTasks { get; set; }
    public int OpenTasks { get; set; }
    public int CompletedTasks { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal PendingPayments { get; set; }
}
