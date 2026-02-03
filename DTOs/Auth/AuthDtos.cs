using Household_Bidding.Models.Enums;

namespace Household_Bidding.DTOs.Auth;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    
    // Profile-specific fields
    public string Address { get; set; } = string.Empty;
    public int CityId { get; set; }
    
    // Worker-specific fields (optional, only for Worker role)
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public decimal? HourlyRate { get; set; }
    public int? ExperienceYears { get; set; }
    public string? Bio { get; set; }
    public List<int>? ServiceCategoryIds { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public bool IsSuspended { get; set; }
    public CustomerProfileDto? CustomerProfile { get; set; }
    public WorkerProfileDto? WorkerProfile { get; set; }
}

public class CustomerProfileDto
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
}

public class WorkerProfileDto
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public decimal HourlyRate { get; set; }
    public int ExperienceYears { get; set; }
    public string Bio { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<ServiceCategoryDto> Skills { get; set; } = new();
}

public class ServiceCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
}
