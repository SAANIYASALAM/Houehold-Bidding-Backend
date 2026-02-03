using Household_Bidding.Data;
using Household_Bidding.DTOs.Worker;
using Household_Bidding.Models.Entities;
using Household_Bidding.Models.Enums;
using Microsoft.EntityFrameworkCore;
using TaskStatusEnum = Household_Bidding.Models.Enums.TaskStatus;

namespace Household_Bidding.Services;

public interface IWorkerService
{
    System.Threading.Tasks.Task<List<NearbyTaskDto>> GetNearbyTasksAsync(int workerProfileId);
    System.Threading.Tasks.Task<WorkerBidDto> CreateBidAsync(int workerProfileId, CreateBidRequest request);
    System.Threading.Tasks.Task<List<WorkerBidDto>> GetMyBidsAsync(int workerProfileId);
    System.Threading.Tasks.Task<WorkerTaskAssignmentDto?> GetActiveTaskAsync(int workerProfileId);
    System.Threading.Tasks.Task<WorkerTaskAssignmentDto> StartTaskAsync(int taskId, int workerProfileId);
    System.Threading.Tasks.Task<WorkerTaskAssignmentDto> CompleteTaskAsync(int taskId, int workerProfileId, bool isComplete);
}

public class WorkerService : IWorkerService
{
    private readonly ApplicationDbContext _context;
    private readonly ILocationService _locationService;
    private const double MaxDistanceKm = 15.0;

    public WorkerService(ApplicationDbContext context, ILocationService locationService)
    {
        _context = context;
        _locationService = locationService;
    }

    public async Task<List<NearbyTaskDto>> GetNearbyTasksAsync(int workerProfileId)
    {
        var workerProfile = await _context.WorkerProfiles
            .Include(wp => wp.City)
            .Include(wp => wp.WorkerSkills)
            .FirstOrDefaultAsync(wp => wp.Id == workerProfileId);

        if (workerProfile == null)
        {
            throw new InvalidOperationException("Worker profile not found");
        }

        // Get worker's skill category IDs
        var skillCategoryIds = workerProfile.WorkerSkills.Select(ws => ws.ServiceCategoryId).ToList();

        // Get open tasks in same city with matching skills
        var tasks = await _context.Tasks
            .Include(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(t => t.ServiceCategory)
            .Include(t => t.Bids)
            .Where(t => 
                t.Status == TaskStatusEnum.Open &&
                t.CityId == workerProfile.CityId &&
                skillCategoryIds.Contains(t.ServiceCategoryId))
            .ToListAsync();

        // Filter by distance and map to DTO
        var nearbyTasks = new List<NearbyTaskDto>();

        foreach (var task in tasks)
        {
            var distance = _locationService.CalculateDistance(
                workerProfile.Latitude,
                workerProfile.Longitude,
                task.Latitude,
                task.Longitude
            );

            if (distance <= MaxDistanceKm)
            {
                nearbyTasks.Add(new NearbyTaskDto
                {
                    Id = task.Id,
                    CustomerName = task.CustomerProfile.User.FullName,
                    CustomerPhoneNumber = task.CustomerProfile.User.PhoneNumber,
                    ServiceCategoryId = task.ServiceCategoryId,
                    ServiceCategoryName = task.ServiceCategory.Name,
                    Title = task.Title,
                    Description = task.Description,
                    Address = task.Address,
                    Latitude = task.Latitude,
                    Longitude = task.Longitude,
                    Budget = task.Budget,
                    ScheduledDate = task.ScheduledDate,
                    DistanceKm = Math.Round(distance, 2),
                    BidsCount = task.Bids.Count,
                    CreatedAt = task.CreatedAt
                });
            }
        }

        return nearbyTasks.OrderBy(t => t.DistanceKm).ToList();
    }

    public async Task<WorkerBidDto> CreateBidAsync(int workerProfileId, CreateBidRequest request)
    {
        // Check if worker has an active task
        var hasActiveTask = await _context.TaskAssignments
            .AnyAsync(ta => ta.WorkerProfileId == workerProfileId && ta.IsActive);

        if (hasActiveTask)
        {
            throw new InvalidOperationException("Cannot bid on tasks while you have an active task");
        }

        var task = await _context.Tasks
            .Include(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId);

        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        if (task.Status != TaskStatusEnum.Open)
        {
            throw new InvalidOperationException("Task is not open for bidding");
        }

        // Check if worker already bid on this task
        var existingBid = await _context.Bids
            .FirstOrDefaultAsync(b => b.TaskId == request.TaskId && b.WorkerProfileId == workerProfileId);

        if (existingBid != null)
        {
            throw new InvalidOperationException("You have already bid on this task");
        }

        var bid = new Bid
        {
            TaskId = request.TaskId,
            WorkerProfileId = workerProfileId,
            ProposedAmount = request.ProposedAmount,
            EstimatedHours = request.EstimatedHours,
            Message = request.Message,
            Status = BidStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Bids.Add(bid);
        await _context.SaveChangesAsync();

        return new WorkerBidDto
        {
            Id = bid.Id,
            TaskId = bid.TaskId,
            TaskTitle = task.Title,
            CustomerName = task.CustomerProfile.User.FullName,
            ProposedAmount = bid.ProposedAmount,
            EstimatedHours = bid.EstimatedHours,
            Message = bid.Message,
            Status = bid.Status,
            CreatedAt = bid.CreatedAt
        };
    }

    public async Task<List<WorkerBidDto>> GetMyBidsAsync(int workerProfileId)
    {
        var bids = await _context.Bids
            .Include(b => b.Task).ThenInclude(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Where(b => b.WorkerProfileId == workerProfileId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bids.Select(b => new WorkerBidDto
        {
            Id = b.Id,
            TaskId = b.TaskId,
            TaskTitle = b.Task.Title,
            CustomerName = b.Task.CustomerProfile.User.FullName,
            ProposedAmount = b.ProposedAmount,
            EstimatedHours = b.EstimatedHours,
            Message = b.Message,
            Status = b.Status,
            CreatedAt = b.CreatedAt
        }).ToList();
    }

    public async Task<WorkerTaskAssignmentDto?> GetActiveTaskAsync(int workerProfileId)
    {
        var assignment = await _context.TaskAssignments
            .Include(ta => ta.Task).ThenInclude(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(ta => ta.Bid)
            .FirstOrDefaultAsync(ta => ta.WorkerProfileId == workerProfileId && ta.IsActive);

        if (assignment == null)
        {
            return null;
        }

        return new WorkerTaskAssignmentDto
        {
            Id = assignment.Id,
            TaskId = assignment.TaskId,
            TaskTitle = assignment.Task.Title,
            TaskDescription = assignment.Task.Description,
            Address = assignment.Task.Address,
            CustomerName = assignment.Task.CustomerProfile.User.FullName,
            CustomerPhoneNumber = assignment.Task.CustomerProfile.User.PhoneNumber,
            TaskStatus = assignment.Task.Status,
            AssignedAt = assignment.AssignedAt,
            StartedAt = assignment.StartedAt,
            CompletedAt = assignment.CompletedAt,
            BidAmount = assignment.Bid.ProposedAmount
        };
    }

    public async Task<WorkerTaskAssignmentDto> StartTaskAsync(int taskId, int workerProfileId)
    {
        var assignment = await _context.TaskAssignments
            .Include(ta => ta.Task).ThenInclude(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(ta => ta.Bid)
            .FirstOrDefaultAsync(ta => ta.TaskId == taskId && ta.WorkerProfileId == workerProfileId && ta.IsActive);

        if (assignment == null)
        {
            throw new InvalidOperationException("Task assignment not found");
        }

        if (assignment.Task.Status != TaskStatusEnum.Assigned)
        {
            throw new InvalidOperationException("Task is not in Assigned status");
        }

        assignment.StartedAt = DateTime.UtcNow;
        assignment.Task.Status = TaskStatusEnum.InProgress;
        assignment.Task.UpdatedAt = DateTime.UtcNow;
        assignment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new WorkerTaskAssignmentDto
        {
            Id = assignment.Id,
            TaskId = assignment.TaskId,
            TaskTitle = assignment.Task.Title,
            TaskDescription = assignment.Task.Description,
            Address = assignment.Task.Address,
            CustomerName = assignment.Task.CustomerProfile.User.FullName,
            CustomerPhoneNumber = assignment.Task.CustomerProfile.User.PhoneNumber,
            TaskStatus = assignment.Task.Status,
            AssignedAt = assignment.AssignedAt,
            StartedAt = assignment.StartedAt,
            CompletedAt = assignment.CompletedAt,
            BidAmount = assignment.Bid.ProposedAmount
        };
    }

    public async Task<WorkerTaskAssignmentDto> CompleteTaskAsync(int taskId, int workerProfileId, bool isComplete)
    {
        var assignment = await _context.TaskAssignments
            .Include(ta => ta.Task).ThenInclude(t => t.CustomerProfile).ThenInclude(cp => cp.User)
            .Include(ta => ta.Bid)
            .FirstOrDefaultAsync(ta => ta.TaskId == taskId && ta.WorkerProfileId == workerProfileId && ta.IsActive);

        if (assignment == null)
        {
            throw new InvalidOperationException("Task assignment not found");
        }

        if (assignment.Task.Status != TaskStatusEnum.InProgress)
        {
            throw new InvalidOperationException("Task is not in InProgress status");
        }

        assignment.CompletedAt = DateTime.UtcNow;
        assignment.Task.Status = isComplete ? TaskStatusEnum.Completed : TaskStatusEnum.Incomplete;
        assignment.Task.UpdatedAt = DateTime.UtcNow;
        assignment.UpdatedAt = DateTime.UtcNow;

        // If task is completed or incomplete (revision resolved), mark assignment as inactive
        if (isComplete)
        {
            assignment.IsActive = false;
        }

        await _context.SaveChangesAsync();

        return new WorkerTaskAssignmentDto
        {
            Id = assignment.Id,
            TaskId = assignment.TaskId,
            TaskTitle = assignment.Task.Title,
            TaskDescription = assignment.Task.Description,
            Address = assignment.Task.Address,
            CustomerName = assignment.Task.CustomerProfile.User.FullName,
            CustomerPhoneNumber = assignment.Task.CustomerProfile.User.PhoneNumber,
            TaskStatus = assignment.Task.Status,
            AssignedAt = assignment.AssignedAt,
            StartedAt = assignment.StartedAt,
            CompletedAt = assignment.CompletedAt,
            BidAmount = assignment.Bid.ProposedAmount
        };
    }
}
