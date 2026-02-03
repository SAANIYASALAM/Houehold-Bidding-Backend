# Household Bidding Backend API

Production-ready ASP.NET Core backend for a household services bidding platform in Kerala, India.

> 📚 **New to this project?** Check out the [Quick Setup Guide](SETUP_GUIDE.md) for fast installation and common commands.

## Tech Stack

- **Backend**: ASP.NET Core (.NET 8)
- **Database**: SQL Server (Entity Framework Core, Code-First)
- **API Style**: REST
- **API Documentation**: Swagger (OpenAPI 3.0)
- **Authentication**: JWT
- **Frontend**: Angular (CORS enabled)

## System Overview

### Scope
- Platform operates ONLY in Kerala, India
- Users belong to major cities/towns in Kerala
- Workers shown ONLY within 15 km radius of a task
- Static worker base locations (no real-time GPS tracking)

### User Roles
1. **Customer** - Post tasks, view bids, assign workers, make payments, leave reviews
2. **Worker** - View nearby tasks, place bids, complete work
3. **Admin** - Monitor platform, manage users, view analytics

## Database Schema

### Entities
- **Users** - User accounts with roles
- **CustomerProfile** - Customer-specific information
- **WorkerProfile** - Worker-specific information with location
- **Cities** - Master data for Kerala cities
- **ServiceCategories** - Master data for service types
- **WorkerSkills** - Many-to-many relationship between workers and services
- **Tasks** - Customer task requests
- **Bids** - Worker bids on tasks
- **TaskAssignments** - Assigned tasks (enforces one active task per worker)
- **TaskRevisions** - Revision requests for incomplete work
- **Payments** - Payment records
- **Reviews** - Customer reviews for completed work

## Business Rules

1. **One Active Task Per Worker**: Workers can only have ONE active task at a time (enforced via filtered unique index)
2. **Task Lifecycle**: OPEN → ASSIGNED → IN_PROGRESS → COMPLETED/INCOMPLETE
3. **Incomplete Tasks**: Allow revision requests before payment
4. **Payment & Reviews**: Only after task completion
5. **Location-based Filtering**: Workers within 15km radius using Haversine distance calculation

## API Endpoints

### Authentication (`/api/Auth`)
- `POST /register` - Register new user (Customer, Worker, or Admin)
- `POST /login` - Login and get JWT token
- `GET /me` - Get current user profile (requires auth)

### Customer (`/api/Customer`)
All endpoints require Customer role authorization.

- `POST /tasks` - Create a new task
- `GET /tasks` - Get all my tasks
- `GET /tasks/{taskId}` - Get specific task details
- `PUT /tasks/{taskId}` - Update a task (Open status only)
- `DELETE /tasks/{taskId}` - Delete a task (Open status only)
- `GET /tasks/{taskId}/bids` - View bids for a task
- `POST /tasks/{taskId}/assign` - Assign worker to task
- `POST /tasks/{taskId}/revision` - Request revision for incomplete task
- `POST /tasks/{taskId}/payment` - Make payment for completed task
- `POST /tasks/{taskId}/review` - Submit review after payment

### Worker (`/api/Worker`)
All endpoints require Worker role authorization.

- `GET /tasks/nearby` - Get tasks within 15km radius (filtered by skills)
- `POST /bids` - Create a bid on a task
- `GET /bids` - Get all my bids
- `GET /tasks/active` - Get current active task
- `POST /tasks/{taskId}/start` - Start working on assigned task
- `POST /tasks/{taskId}/complete?isComplete=true` - Complete task (or mark incomplete)

### Admin (`/api/Admin`)
All endpoints require Admin role authorization.

- `GET /users` - Get all users
- `GET /tasks` - Get all tasks
- `GET /payments` - Get all payments
- `GET /reviews` - Get all reviews
- `POST /users/{userId}/suspend` - Suspend/unsuspend a user
- `GET /dashboard/stats` - Get platform statistics

### Master Data (`/api/MasterData`)
Public endpoints.

- `GET /cities` - Get all Kerala cities
- `GET /service-categories` - Get all service categories

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code
- Entity Framework Core Tools (install below)

### Setup

1. **Clone the repository**
```bash
git clone <repository-url>
cd Houehold-Bidding-Backend
```

2. **Install EF Core Tools** (Required for database migrations)
```bash
dotnet tool install --global dotnet-ef
```

If already installed, update to the latest version:
```bash
dotnet tool update --global dotnet-ef
```

Verify installation:
```bash
dotnet ef --version
```

3. **Update Connection String**
Edit `appsettings.json` and update the connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HouseholdBidding;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

4. **Update JWT Settings (Production)**
For production, change the `SecretKey` in `appsettings.json`:
```json
"JwtSettings": {
  "SecretKey": "YOUR_SECURE_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
  "Issuer": "HouseholdBidding",
  "Audience": "HouseholdBiddingClient"
}
```

5. **Create Database**

**Option 1: Using .NET CLI (Recommended)**
```bash
dotnet ef database update
```

**Option 2: Using Package Manager Console (Visual Studio)**

First, ensure EF Core tools are installed (see step 2), then in Package Manager Console:
```powershell
# If you prefer PowerShell commands in VS, use:
dotnet ef database update

# Note: The 'Update-Database' command is a PowerShell-specific command 
# that requires the older EF6 tools or special configuration.
# For EF Core projects, use 'dotnet ef database update' instead.
```

6. **Run the Application**
```bash
dotnet run
```

7. **Access Swagger UI**
Navigate to: `https://localhost:<port>/` or `http://localhost:<port>/`

## Features

### Location-based Task Discovery
- Workers see only tasks within 15km radius
- Uses Haversine distance calculation
- Filtered by city and worker skills

### JWT Authentication
- Token-based authentication
- Role-based authorization (Customer, Worker, Admin)
- Swagger UI includes JWT authentication support

### Business Logic Enforcement
- Filtered unique index ensures one active task per worker
- Task status validation for all operations
- Payment requires task completion
- Review requires payment completion

### CORS Configuration
- Configured for Angular frontend on `http://localhost:4200`
- Supports credentials for authenticated requests

## Master Data

### Kerala Cities (Seeded)
- Thiruvananthapuram
- Kochi
- Kozhikode
- Thrissur
- Kollam
- Alappuzha
- Palakkad
- Kannur
- Kottayam
- Malappuram

### Service Categories (Seeded)
- Electrician
- Plumber
- Carpenter
- AC Repair
- House Cleaning
- Painter
- Pest Control
- Appliance Repair

## Development

### Project Structure
```
├── Controllers/          # API Controllers
├── Data/                # DbContext and Seeder
├── DTOs/                # Data Transfer Objects
│   ├── Auth/
│   ├── Customer/
│   ├── Worker/
│   ├── Admin/
│   └── MasterData/
├── Models/
│   ├── Entities/        # EF Core Entities
│   └── Enums/           # Enumerations
├── Services/            # Business Logic Services
└── Migrations/          # EF Core Migrations
```

### Adding Migrations
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Troubleshooting

### Issue: `update-database` command not recognized

**Error Message:**
```
The term 'update-database' is not recognized as the name of a cmdlet, function, script file, or operable program.
```

**Solution:**

The `Update-Database` command is from Entity Framework 6 (EF6), but this project uses Entity Framework Core (EF Core), which has different command-line tools.

**To fix this:**

1. **Install EF Core Tools globally** (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. **Use the correct command** for EF Core:
   ```bash
   # In terminal/command prompt:
   dotnet ef database update
   
   # In Package Manager Console (Visual Studio):
   dotnet ef database update
   ```

3. **Verify the tools are installed:**
   ```bash
   dotnet ef --version
   ```

**Why this happens:**
- `Update-Database` is for EF6 (older version)
- `dotnet ef database update` is for EF Core (current version)
- This project uses EF Core 8.0, so you must use `dotnet ef` commands

### Issue: `dotnet-ef` not found

**Solution:**
Install the EF Core tools globally:
```bash
dotnet tool install --global dotnet-ef
```

Or update if already installed:
```bash
dotnet tool update --global dotnet-ef
```

## Security Notes

⚠️ **Important for Production**:
1. Change JWT `SecretKey` to a secure random string
2. Use HTTPS in production
3. Update CORS policy to match your frontend domain
4. Store connection strings and secrets in Azure Key Vault or environment variables
5. Enable proper logging and monitoring
6. Implement rate limiting for API endpoints

## License

This project is for educational/demonstration purposes.
