using Household_Bidding.Data;
using Household_Bidding.DTOs.Customer;
using Household_Bidding.Models.Entities;
using Household_Bidding.Models.Enums;
using Microsoft.EntityFrameworkCore;
using TaskEntity = Household_Bidding.Models.Entities.Task;
using TaskStatusEnum = Household_Bidding.Models.Enums.TaskStatus;

namespace Household_Bidding.Services;

public interface ICustomerService
{
    System.Threading.Tasks.Task<TaskDto> CreateTaskAsync(int customerProfileId, CreateTaskRequest request);
    System.Threading.Tasks.Task<TaskDto> UpdateTaskAsync(int taskId, int customerProfileId, UpdateTaskRequest request);
    System.Threading.Tasks.Task DeleteTaskAsync(int taskId, int customerProfileId);
    System.Threading.Tasks.Task<List<TaskDto>> GetMyTasksAsync(int customerProfileId);
    System.Threading.Tasks.Task<TaskDto> GetTaskByIdAsync(int taskId, int customerProfileId);
    System.Threading.Tasks.Task<List<BidDto>> GetTaskBidsAsync(int taskId, int customerProfileId);
    System.Threading.Tasks.Task<TaskDto> AssignWorkerAsync(int taskId, int customerProfileId, AssignWorkerRequest request);
    System.Threading.Tasks.Task<TaskRevisionDto> RequestRevisionAsync(int taskId, int customerProfileId, TaskRevisionRequest request);
    System.Threading.Tasks.Task<PaymentDto> MakePaymentAsync(int taskId, int customerProfileId, PaymentRequest request);
    System.Threading.Tasks.Task<ReviewDto> CreateReviewAsync(int taskId, int customerProfileId, ReviewRequest request);
}

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskDto> CreateTaskAsync(int customerProfileId, CreateTaskRequest request)
    {
        var customerProfile = await _context.CustomerProfiles
            .Include(cp => cp.City)
            .FirstOrDefaultAsync(cp => cp.Id == customerProfileId);

        if (customerProfile == null)
        {
            throw new InvalidOperationException("Customer profile not found");
        }

        var task = new TaskEntity
        {
            CustomerProfileId = customerProfileId,
            ServiceCategoryId = request.ServiceCategoryId,
            CityId = customerProfile.CityId,
            Title = request.Title,
            Description = request.Description,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Budget = request.Budget,
            ScheduledDate = request.ScheduledDate,
            Status = TaskStatusEnum.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return await GetTaskDtoAsync(task.Id);
    }

    public async Task<TaskDto> UpdateTaskAsync(int taskId, int customerProfileId, UpdateTaskRequest request)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CustomerProfileId == customerProfileId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        if (task.Status != TaskStatusEnum.Open)
        {
            throw new InvalidOperationException("Can only update tasks in Open status");
        }

        if (request.Title != null) task.Title = request.Title;
        if (request.Description != null) task.Description = request.Description;
        if (request.Address != null) task.Address = request.Address;
        if (request.Latitude.HasValue) task.Latitude = request.Latitude.Value;
        if (request.Longitude.HasValue) task.Longitude = request.Longitude.Value;
        if (request.Budget.HasValue) task.Budget = request.Budget.Value;
        if (request.ScheduledDate.HasValue) task.ScheduledDate = request.ScheduledDate.Value;

        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GetTaskDtoAsync(task.Id);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int taskId, int customerProfileId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CustomerProfileId == customerProfileId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        if (task.Status != TaskStatusEnum.Open)
        {
            throw new InvalidOperationException("Can only delete tasks in Open status");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<List<TaskDto>> GetMyTasksAsync(int customerProfileId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(t => t.ServiceCategory)
            .Include(t => t.City)
            .Include(t => t.Bids)
            .Where(t => t.CustomerProfileId == customerProfileId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(t => MapToTaskDto(t)).ToList();
    }

    public async Task<TaskDto> GetTaskByIdAsync(int taskId, int customerProfileId)
    {
        var task = await _context.Tasks
            .Include(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(t => t.ServiceCategory)
            .Include(t => t.City)
            .Include(t => t.Bids)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CustomerProfileId == customerProfileId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        return MapToTaskDto(task);
    }

    public async Task<List<BidDto>> GetTaskBidsAsync(int taskId, int customerProfileId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CustomerProfileId == customerProfileId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        var bids = await _context.Bids
            .Include(b => b.WorkerProfile).ThenInclude(wp => wp.User)
            .Where(b => b.TaskId == taskId)
            .OrderBy(b => b.ProposedAmount)
            .ToListAsync();

        return bids.Select(b => new BidDto
        {
            Id = b.Id,
            TaskId = b.TaskId,
            WorkerProfileId = b.WorkerProfileId,
            WorkerName = b.WorkerProfile.User.FullName,
            WorkerPhoneNumber = b.WorkerProfile.User.PhoneNumber,
            WorkerRating = b.WorkerProfile.AverageRating,
            WorkerTotalReviews = b.WorkerProfile.TotalReviews,
            WorkerExperienceYears = b.WorkerProfile.ExperienceYears,
            ProposedAmount = b.ProposedAmount,
            EstimatedHours = b.EstimatedHours,
            Message = b.Message,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        }).ToList();
    }

    public async Task<TaskDto> AssignWorkerAsync(int taskId, int customerProfileId, AssignWorkerRequest request)
    {
        var task = await _context.Tasks
            .Include(t => t.TaskAssignment)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CustomerProfileId == customerProfileId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        if (task.Status != TaskStatusEnum.Open)
        {
            throw new InvalidOperationException("Task is not in Open status");
        }

        var bid = await _context.Bids
            .Include(b => b.WorkerProfile)
            .FirstOrDefaultAsync(b => b.Id == request.BidId && b.TaskId == taskId);

        if (bid == null)
        {
            throw new InvalidOperationException("Bid not found");
        }

        // Check if worker has an active task
        var hasActiveTask = await _context.TaskAssignments
            .AnyAsync(ta => ta.WorkerProfileId == bid.WorkerProfileId && ta.IsActive);

        if (hasActiveTask)
        {
            throw new InvalidOperationException("Worker already has an active task");
        }

        // Update task status
        task.Status = TaskStatusEnum.Assigned;
        task.UpdatedAt = DateTime.UtcNow;

        // Update bid status
        bid.Status = BidStatus.Accepted;
        bid.UpdatedAt = DateTime.UtcNow;

        // Reject other bids
        var otherBids = await _context.Bids
            .Where(b => b.TaskId == taskId && b.Id != bid.Id && b.Status == BidStatus.Pending)
            .ToListAsync();

        foreach (var otherBid in otherBids)
        {
            otherBid.Status = BidStatus.Rejected;
            otherBid.UpdatedAt = DateTime.UtcNow;
        }

        // Create task assignment
        var assignment = new TaskAssignment
        {
            TaskId = taskId,
            WorkerProfileId = bid.WorkerProfileId,
            BidId = bid.Id,
            AssignedAt = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return await GetTaskDtoAsync(taskId);
    }

    public async Task<TaskRevisionDto> RequestRevisionAsync(int taskId, int customerProfileId, TaskRevisionRequest request)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CustomerProfileId == customerProfileId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        if (task.Status != TaskStatusEnum.Incomplete)
        {
            throw new InvalidOperationException("Can only request revision for incomplete tasks");
        }

        var revision = new TaskRevision
        {
            TaskId = taskId,
            Reason = request.Reason,
            Description = request.Description,
            IsResolved = false,
            RequestedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TaskRevisions.Add(revision);
        await _context.SaveChangesAsync();

        return new TaskRevisionDto
        {
            Id = revision.Id,
            TaskId = revision.TaskId,
            Reason = revision.Reason,
            Description = revision.Description,
            IsResolved = revision.IsResolved,
            Resolution = revision.Resolution,
            RequestedAt = revision.RequestedAt,
            ResolvedAt = revision.ResolvedAt
        };
    }

    public async Task<PaymentDto> MakePaymentAsync(int taskId, int customerProfileId, PaymentRequest request)
    {
        var task = await _context.Tasks
            .Include(t => t.TaskAssignment)
            .Include(t => t.Payment)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CustomerProfileId == customerProfileId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        if (task.Status != TaskStatusEnum.Completed)
        {
            throw new InvalidOperationException("Can only make payment for completed tasks");
        }

        if (task.Payment != null)
        {
            throw new InvalidOperationException("Payment already exists for this task");
        }

        if (task.TaskAssignment == null)
        {
            throw new InvalidOperationException("No worker assigned to this task");
        }

        var bid = await _context.Bids.FindAsync(task.TaskAssignment.BidId);
        if (bid == null)
        {
            throw new InvalidOperationException("Bid not found");
        }

        var payment = new Payment
        {
            TaskId = taskId,
            CustomerProfileId = customerProfileId,
            WorkerProfileId = task.TaskAssignment.WorkerProfileId,
            Amount = bid.ProposedAmount,
            Status = PaymentStatus.Completed,
            TransactionId = request.TransactionId,
            PaymentMethod = request.PaymentMethod,
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return new PaymentDto
        {
            Id = payment.Id,
            TaskId = payment.TaskId,
            Amount = payment.Amount,
            Status = payment.Status,
            TransactionId = payment.TransactionId,
            PaymentMethod = payment.PaymentMethod,
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<ReviewDto> CreateReviewAsync(int taskId, int customerProfileId, ReviewRequest request)
    {
        var task = await _context.Tasks
            .Include(t => t.TaskAssignment)
            .Include(t => t.Review)
            .Include(t => t.Payment)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.CustomerProfileId == customerProfileId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        if (task.Status != TaskStatusEnum.Completed)
        {
            throw new InvalidOperationException("Can only review completed tasks");
        }

        if (task.Payment == null)
        {
            throw new InvalidOperationException("Payment must be made before review");
        }

        if (task.Review != null)
        {
            throw new InvalidOperationException("Review already exists for this task");
        }

        if (task.TaskAssignment == null)
        {
            throw new InvalidOperationException("No worker assigned to this task");
        }

        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new InvalidOperationException("Rating must be between 1 and 5");
        }

        var review = new Review
        {
            TaskId = taskId,
            CustomerProfileId = customerProfileId,
            WorkerProfileId = task.TaskAssignment.WorkerProfileId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);

        // Update worker's average rating
        var workerProfile = await _context.WorkerProfiles
            .Include(wp => wp.ReceivedReviews)
            .FirstOrDefaultAsync(wp => wp.Id == task.TaskAssignment.WorkerProfileId);

        if (workerProfile != null)
        {
            var allReviews = workerProfile.ReceivedReviews.ToList();
            allReviews.Add(review);
            workerProfile.AverageRating = allReviews.Average(r => r.Rating);
            workerProfile.TotalReviews = allReviews.Count;
            workerProfile.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var workerUser = await _context.Users.FindAsync(workerProfile?.UserId);

        return new ReviewDto
        {
            Id = review.Id,
            TaskId = review.TaskId,
            WorkerProfileId = review.WorkerProfileId,
            WorkerName = workerUser?.FullName ?? string.Empty,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    private async Task<TaskDto> GetTaskDtoAsync(int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(t => t.ServiceCategory)
            .Include(t => t.City)
            .Include(t => t.Bids)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        return MapToTaskDto(task);
    }

    private TaskDto MapToTaskDto(TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            CustomerProfileId = task.CustomerProfileId,
            CustomerName = task.CustomerProfile.User.FullName,
            ServiceCategoryId = task.ServiceCategoryId,
            ServiceCategoryName = task.ServiceCategory.Name,
            CityId = task.CityId,
            CityName = task.City.Name,
            Title = task.Title,
            Description = task.Description,
            Address = task.Address,
            Latitude = task.Latitude,
            Longitude = task.Longitude,
            Budget = task.Budget,
            ScheduledDate = task.ScheduledDate,
            Status = task.Status,
            BidsCount = task.Bids.Count,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
