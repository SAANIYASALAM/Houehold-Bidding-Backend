using Household_Bidding.Data;
using Household_Bidding.DTOs.Admin;
using Household_Bidding.Models.Enums;
using Microsoft.EntityFrameworkCore;
using TaskStatusEnum = Household_Bidding.Models.Enums.TaskStatus;

namespace Household_Bidding.Services;

public interface IAdminService
{
    System.Threading.Tasks.Task<List<UserManagementDto>> GetAllUsersAsync();
    System.Threading.Tasks.Task<List<TaskManagementDto>> GetAllTasksAsync();
    System.Threading.Tasks.Task<List<PaymentManagementDto>> GetAllPaymentsAsync();
    System.Threading.Tasks.Task<List<ReviewManagementDto>> GetAllReviewsAsync();
    System.Threading.Tasks.Task SuspendUserAsync(int userId, bool suspend);
    System.Threading.Tasks.Task<DashboardStatsDto> GetDashboardStatsAsync();
}

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserManagementDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(u => new UserManagementDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role,
            IsActive = u.IsActive,
            IsSuspended = u.IsSuspended,
            CreatedAt = u.CreatedAt
        }).ToList();
    }

    public async Task<List<TaskManagementDto>> GetAllTasksAsync()
    {
        var tasks = await _context.Tasks
            .Include(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(t => t.ServiceCategory)
            .Include(t => t.TaskAssignment).ThenInclude(ta => ta!.WorkerProfile).ThenInclude(wp => wp.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(t => new TaskManagementDto
        {
            Id = t.Id,
            Title = t.Title,
            CustomerName = t.CustomerProfile.User.FullName,
            ServiceCategoryName = t.ServiceCategory.Name,
            Status = t.Status,
            Budget = t.Budget,
            ScheduledDate = t.ScheduledDate,
            CreatedAt = t.CreatedAt,
            WorkerName = t.TaskAssignment?.WorkerProfile?.User?.FullName
        }).ToList();
    }

    public async Task<List<PaymentManagementDto>> GetAllPaymentsAsync()
    {
        var payments = await _context.Payments
            .Include(p => p.Task)
            .Include(p => p.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(p => p.WorkerProfile).ThenInclude(wp => wp.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return payments.Select(p => new PaymentManagementDto
        {
            Id = p.Id,
            TaskId = p.TaskId,
            TaskTitle = p.Task.Title,
            CustomerName = p.CustomerProfile.User.FullName,
            WorkerName = p.WorkerProfile.User.FullName,
            Amount = p.Amount,
            Status = p.Status,
            PaidAt = p.PaidAt,
            CreatedAt = p.CreatedAt
        }).ToList();
    }

    public async Task<List<ReviewManagementDto>> GetAllReviewsAsync()
    {
        var reviews = await _context.Reviews
            .Include(r => r.Task)
            .Include(r => r.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(r => r.WorkerProfile).ThenInclude(wp => wp.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(r => new ReviewManagementDto
        {
            Id = r.Id,
            TaskId = r.TaskId,
            TaskTitle = r.Task.Title,
            CustomerName = r.CustomerProfile.User.FullName,
            WorkerName = r.WorkerProfile.User.FullName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task SuspendUserAsync(int userId, bool suspend)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (user.Role == UserRole.Admin)
        {
            throw new InvalidOperationException("Cannot suspend admin users");
        }

        user.IsSuspended = suspend;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalCustomers = await _context.Users.CountAsync(u => u.Role == UserRole.Customer);
        var totalWorkers = await _context.Users.CountAsync(u => u.Role == UserRole.Worker);
        var totalTasks = await _context.Tasks.CountAsync();
        var openTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatusEnum.Open);
        var completedTasks = await _context.Tasks.CountAsync(t => t.Status == TaskStatusEnum.Completed);
        var totalPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed)
            .SumAsync(p => (decimal?)p.Amount) ?? 0;
        var pendingPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Pending)
            .SumAsync(p => (decimal?)p.Amount) ?? 0;

        return new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            TotalCustomers = totalCustomers,
            TotalWorkers = totalWorkers,
            TotalTasks = totalTasks,
            OpenTasks = openTasks,
            CompletedTasks = completedTasks,
            TotalPayments = totalPayments,
            PendingPayments = pendingPayments
        };
    }
}
