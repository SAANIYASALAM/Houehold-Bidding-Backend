using Household_Bidding.Data;
using Household_Bidding.Models.Entities;

namespace Household_Bidding.Data;

public static class DataSeeder
{
    public static void SeedData(ApplicationDbContext context)
    {
        // Seed Cities (Kerala, India)
        if (!context.Cities.Any())
        {
            var cities = new List<City>
            {
                new City { Name = "Thiruvananthapuram", State = "Kerala", Latitude = 8.5241, Longitude = 76.9366, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Kochi", State = "Kerala", Latitude = 9.9312, Longitude = 76.2673, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Kozhikode", State = "Kerala", Latitude = 11.2588, Longitude = 75.7804, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Thrissur", State = "Kerala", Latitude = 10.5276, Longitude = 76.2144, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Kollam", State = "Kerala", Latitude = 8.8932, Longitude = 76.6141, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Alappuzha", State = "Kerala", Latitude = 9.4981, Longitude = 76.3388, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Palakkad", State = "Kerala", Latitude = 10.7867, Longitude = 76.6548, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Kannur", State = "Kerala", Latitude = 11.8745, Longitude = 75.3704, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Kottayam", State = "Kerala", Latitude = 9.5916, Longitude = 76.5222, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Malappuram", State = "Kerala", Latitude = 11.0510, Longitude = 76.0711, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Cities.AddRange(cities);
            context.SaveChanges();
        }

        // Seed Service Categories
        if (!context.ServiceCategories.Any())
        {
            var serviceCategories = new List<ServiceCategory>
            {
                new ServiceCategory 
                { 
                    Name = "Electrician", 
                    Description = "Electrical repairs and installations",
                    IconUrl = "/icons/electrician.png",
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new ServiceCategory 
                { 
                    Name = "Plumber", 
                    Description = "Plumbing repairs and installations",
                    IconUrl = "/icons/plumber.png",
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new ServiceCategory 
                { 
                    Name = "Carpenter", 
                    Description = "Carpentry and woodworking services",
                    IconUrl = "/icons/carpenter.png",
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new ServiceCategory 
                { 
                    Name = "AC Repair", 
                    Description = "Air conditioning repair and maintenance",
                    IconUrl = "/icons/ac-repair.png",
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new ServiceCategory 
                { 
                    Name = "House Cleaning", 
                    Description = "Professional house cleaning services",
                    IconUrl = "/icons/cleaning.png",
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new ServiceCategory 
                { 
                    Name = "Painter", 
                    Description = "Painting and wall finishing services",
                    IconUrl = "/icons/painter.png",
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new ServiceCategory 
                { 
                    Name = "Pest Control", 
                    Description = "Pest control and fumigation services",
                    IconUrl = "/icons/pest-control.png",
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new ServiceCategory 
                { 
                    Name = "Appliance Repair", 
                    Description = "Home appliance repair services",
                    IconUrl = "/icons/appliance-repair.png",
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                }
            };

            context.ServiceCategories.AddRange(serviceCategories);
            context.SaveChanges();
        }
    }
}
