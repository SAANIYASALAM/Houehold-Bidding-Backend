using Household_Bidding.Data;
using Microsoft.EntityFrameworkCore;

namespace Household_Bidding.Services;

public interface IProfileService
{
    System.Threading.Tasks.Task<int?> GetCustomerProfileIdAsync(int userId);
    System.Threading.Tasks.Task<int?> GetWorkerProfileIdAsync(int userId);
}

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _context;

    public ProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<int?> GetCustomerProfileIdAsync(int userId)
    {
        var profile = await _context.CustomerProfiles
            .FirstOrDefaultAsync(cp => cp.UserId == userId);
        return profile?.Id;
    }

    public async System.Threading.Tasks.Task<int?> GetWorkerProfileIdAsync(int userId)
    {
        var profile = await _context.WorkerProfiles
            .FirstOrDefaultAsync(wp => wp.UserId == userId);
        return profile?.Id;
    }
}
