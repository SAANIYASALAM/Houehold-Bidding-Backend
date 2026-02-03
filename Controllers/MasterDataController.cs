using Household_Bidding.Data;
using Household_Bidding.DTOs.MasterData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Household_Bidding.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasterDataController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MasterDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all cities in Kerala
    /// </summary>
    [HttpGet("cities")]
    public async Task<ActionResult<List<CityDto>>> GetCities()
    {
        try
        {
            var cities = await _context.Cities
                .OrderBy(c => c.Name)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    State = c.State,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                })
                .ToListAsync();

            return Ok(cities);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all service categories
    /// </summary>
    [HttpGet("service-categories")]
    public async Task<ActionResult<List<ServiceCategoryDto>>> GetServiceCategories()
    {
        try
        {
            var categories = await _context.ServiceCategories
                .OrderBy(sc => sc.Name)
                .Select(sc => new ServiceCategoryDto
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    Description = sc.Description,
                    IconUrl = sc.IconUrl
                })
                .ToListAsync();

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
