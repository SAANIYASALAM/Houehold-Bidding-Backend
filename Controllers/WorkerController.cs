using Household_Bidding.DTOs.Worker;
using Household_Bidding.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Household_Bidding.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Worker")]
public class WorkerController : ControllerBase
{
    private readonly IWorkerService _workerService;
    private readonly IProfileService _profileService;

    public WorkerController(IWorkerService workerService, IProfileService profileService)
    {
        _workerService = workerService;
        _profileService = profileService;
    }

    private async System.Threading.Tasks.Task<int> GetWorkerProfileIdAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
        
        var profileId = await _profileService.GetWorkerProfileIdAsync(userId);
        if (profileId == null)
        {
            throw new InvalidOperationException("Worker profile not found");
        }
        
        return profileId.Value;
    }

    /// <summary>
    /// Get nearby tasks within 15km radius
    /// </summary>
    [HttpGet("tasks/nearby")]
    public async System.Threading.Tasks.Task<ActionResult<List<NearbyTaskDto>>> GetNearbyTasks()
    {
        try
        {
            var workerProfileId = await GetWorkerProfileIdAsync();
            var tasks = await _workerService.GetNearbyTasksAsync(workerProfileId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a bid on a task
    /// </summary>
    [HttpPost("bids")]
    public async System.Threading.Tasks.Task<ActionResult<WorkerBidDto>> CreateBid([FromBody] CreateBidRequest request)
    {
        try
        {
            var workerProfileId = await GetWorkerProfileIdAsync();
            var bid = await _workerService.CreateBidAsync(workerProfileId, request);
            return Ok(bid);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all my bids
    /// </summary>
    [HttpGet("bids")]
    public async System.Threading.Tasks.Task<ActionResult<List<WorkerBidDto>>> GetMyBids()
    {
        try
        {
            var workerProfileId = await GetWorkerProfileIdAsync();
            var bids = await _workerService.GetMyBidsAsync(workerProfileId);
            return Ok(bids);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get current active task
    /// </summary>
    [HttpGet("tasks/active")]
    public async System.Threading.Tasks.Task<ActionResult<WorkerTaskAssignmentDto>> GetActiveTask()
    {
        try
        {
            var workerProfileId = await GetWorkerProfileIdAsync();
            var task = await _workerService.GetActiveTaskAsync(workerProfileId);
            if (task == null)
            {
                return NotFound(new { message = "No active task" });
            }
            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Start work on assigned task
    /// </summary>
    [HttpPost("tasks/{taskId}/start")]
    public async System.Threading.Tasks.Task<ActionResult<WorkerTaskAssignmentDto>> StartTask(int taskId)
    {
        try
        {
            var workerProfileId = await GetWorkerProfileIdAsync();
            var task = await _workerService.StartTaskAsync(taskId, workerProfileId);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Complete task (mark as complete or incomplete)
    /// </summary>
    [HttpPost("tasks/{taskId}/complete")]
    public async System.Threading.Tasks.Task<ActionResult<WorkerTaskAssignmentDto>> CompleteTask(int taskId, [FromQuery] bool isComplete = true)
    {
        try
        {
            var workerProfileId = await GetWorkerProfileIdAsync();
            var task = await _workerService.CompleteTaskAsync(taskId, workerProfileId, isComplete);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
