using Household_Bidding.Data;
using Household_Bidding.DTOs.Auth;
using Household_Bidding.Models.Entities;
using Household_Bidding.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Household_Bidding.Services;

public interface IAuthService
{
    System.Threading.Tasks.Task<UserDto> RegisterAsync(RegisterRequest request);
    System.Threading.Tasks.Task<LoginResponse> LoginAsync(LoginRequest request);
    System.Threading.Tasks.Task<UserDto?> GetCurrentUserAsync(int userId);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Create user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            IsActive = true,
            IsSuspended = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Create profile based on role
        if (request.Role == UserRole.Customer)
        {
            var customerProfile = new CustomerProfile
            {
                UserId = user.Id,
                Address = request.Address,
                CityId = request.CityId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.CustomerProfiles.Add(customerProfile);
        }
        else if (request.Role == UserRole.Worker)
        {
            var workerProfile = new WorkerProfile
            {
                UserId = user.Id,
                Address = request.Address,
                CityId = request.CityId,
                Latitude = request.Latitude ?? 0,
                Longitude = request.Longitude ?? 0,
                HourlyRate = request.HourlyRate ?? 0,
                ExperienceYears = request.ExperienceYears ?? 0,
                Bio = request.Bio ?? string.Empty,
                AverageRating = 0,
                TotalReviews = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.WorkerProfiles.Add(workerProfile);
            await _context.SaveChangesAsync();

            // Add worker skills
            if (request.ServiceCategoryIds != null && request.ServiceCategoryIds.Any())
            {
                foreach (var categoryId in request.ServiceCategoryIds)
                {
                    var skill = new WorkerSkill
                    {
                        WorkerProfileId = workerProfile.Id,
                        ServiceCategoryId = categoryId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.WorkerSkills.Add(skill);
                }
            }
        }

        await _context.SaveChangesAsync();

        // Return user DTO
        return await GetUserDtoAsync(user.Id);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (user.IsSuspended)
        {
            throw new UnauthorizedAccessException("Account is suspended");
        }

        var token = _tokenService.GenerateToken(user);
        var userDto = await GetUserDtoAsync(user.Id);

        return new LoginResponse
        {
            Token = token,
            User = userDto
        };
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        return await GetUserDtoAsync(userId);
    }

    private async Task<UserDto> GetUserDtoAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.CustomerProfile)
                .ThenInclude(cp => cp!.City)
            .Include(u => u.WorkerProfile)
                .ThenInclude(wp => wp!.City)
            .Include(u => u.WorkerProfile)
                .ThenInclude(wp => wp!.WorkerSkills)
                .ThenInclude(ws => ws.ServiceCategory)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsActive = user.IsActive,
            IsSuspended = user.IsSuspended
        };

        if (user.CustomerProfile != null)
        {
            userDto.CustomerProfile = new CustomerProfileDto
            {
                Id = user.CustomerProfile.Id,
                Address = user.CustomerProfile.Address,
                CityId = user.CustomerProfile.CityId,
                CityName = user.CustomerProfile.City.Name
            };
        }

        if (user.WorkerProfile != null)
        {
            userDto.WorkerProfile = new WorkerProfileDto
            {
                Id = user.WorkerProfile.Id,
                Address = user.WorkerProfile.Address,
                CityId = user.WorkerProfile.CityId,
                CityName = user.WorkerProfile.City.Name,
                Latitude = user.WorkerProfile.Latitude,
                Longitude = user.WorkerProfile.Longitude,
                HourlyRate = user.WorkerProfile.HourlyRate,
                ExperienceYears = user.WorkerProfile.ExperienceYears,
                Bio = user.WorkerProfile.Bio,
                AverageRating = user.WorkerProfile.AverageRating,
                TotalReviews = user.WorkerProfile.TotalReviews,
                Skills = user.WorkerProfile.WorkerSkills
                    .Select(ws => new ServiceCategoryDto
                    {
                        Id = ws.ServiceCategory.Id,
                        Name = ws.ServiceCategory.Name,
                        Description = ws.ServiceCategory.Description,
                        IconUrl = ws.ServiceCategory.IconUrl
                    }).ToList()
            };
        }

        return userDto;
    }
}
