using Household_Bidding.DTOs.Admin;
using Household_Bidding.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Household_Bidding.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult<List<UserManagementDto>>> GetAllUsers()
    {
        try
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    [HttpGet("tasks")]
    public async Task<ActionResult<List<TaskManagementDto>>> GetAllTasks()
    {
        try
        {
            var tasks = await _adminService.GetAllTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all payments
    /// </summary>
    [HttpGet("payments")]
    public async Task<ActionResult<List<PaymentManagementDto>>> GetAllPayments()
    {
        try
        {
            var payments = await _adminService.GetAllPaymentsAsync();
            return Ok(payments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all reviews
    /// </summary>
    [HttpGet("reviews")]
    public async Task<ActionResult<List<ReviewManagementDto>>> GetAllReviews()
    {
        try
        {
            var reviews = await _adminService.GetAllReviewsAsync();
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Suspend or unsuspend a user
    /// </summary>
    [HttpPost("users/{userId}/suspend")]
    public async Task<ActionResult> SuspendUser(int userId, [FromBody] SuspendUserRequest request)
    {
        try
        {
            await _adminService.SuspendUserAsync(userId, request.Suspend);
            return Ok(new { message = request.Suspend ? "User suspended" : "User unsuspended" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get dashboard statistics
    /// </summary>
    [HttpGet("dashboard/stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        try
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
