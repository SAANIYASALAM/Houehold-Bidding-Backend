using Household_Bidding.DTOs.Customer;
using Household_Bidding.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Household_Bidding.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    private int GetCustomerProfileId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid token");
        }
        // In a real scenario, we'd fetch the customer profile ID from the user ID
        // For simplicity, assuming the claim contains profile information
        return userId; // This would be replaced with actual profile ID lookup
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost("tasks")]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskRequest request)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var task = await _customerService.CreateTaskAsync(customerProfileId, request);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a task
    /// </summary>
    [HttpPut("tasks/{taskId}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(int taskId, [FromBody] UpdateTaskRequest request)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var task = await _customerService.UpdateTaskAsync(taskId, customerProfileId, request);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("tasks/{taskId}")]
    public async Task<ActionResult> DeleteTask(int taskId)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            await _customerService.DeleteTaskAsync(taskId, customerProfileId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all my tasks
    /// </summary>
    [HttpGet("tasks")]
    public async Task<ActionResult<List<TaskDto>>> GetMyTasks()
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var tasks = await _customerService.GetMyTasksAsync(customerProfileId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    [HttpGet("tasks/{taskId}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(int taskId)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var task = await _customerService.GetTaskByIdAsync(taskId, customerProfileId);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get bids for a specific task
    /// </summary>
    [HttpGet("tasks/{taskId}/bids")]
    public async Task<ActionResult<List<BidDto>>> GetTaskBids(int taskId)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var bids = await _customerService.GetTaskBidsAsync(taskId, customerProfileId);
            return Ok(bids);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Assign a worker to a task
    /// </summary>
    [HttpPost("tasks/{taskId}/assign")]
    public async Task<ActionResult<TaskDto>> AssignWorker(int taskId, [FromBody] AssignWorkerRequest request)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var task = await _customerService.AssignWorkerAsync(taskId, customerProfileId, request);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Request a revision for incomplete task
    /// </summary>
    [HttpPost("tasks/{taskId}/revision")]
    public async Task<ActionResult<TaskRevisionDto>> RequestRevision(int taskId, [FromBody] TaskRevisionRequest request)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var revision = await _customerService.RequestRevisionAsync(taskId, customerProfileId, request);
            return Ok(revision);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Make payment for completed task
    /// </summary>
    [HttpPost("tasks/{taskId}/payment")]
    public async Task<ActionResult<PaymentDto>> MakePayment(int taskId, [FromBody] PaymentRequest request)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var payment = await _customerService.MakePaymentAsync(taskId, customerProfileId, request);
            return Ok(payment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create review for completed task
    /// </summary>
    [HttpPost("tasks/{taskId}/review")]
    public async Task<ActionResult<ReviewDto>> CreateReview(int taskId, [FromBody] ReviewRequest request)
    {
        try
        {
            var customerProfileId = GetCustomerProfileId();
            var review = await _customerService.CreateReviewAsync(taskId, customerProfileId, request);
            return Ok(review);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
