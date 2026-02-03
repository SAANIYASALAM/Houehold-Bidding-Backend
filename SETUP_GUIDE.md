# Quick Setup Guide

## First-Time Setup

### 1. Install Prerequisites

```bash
# Verify .NET 8 SDK is installed
dotnet --version

# Install EF Core Tools (REQUIRED)
dotnet tool install --global dotnet-ef

# Verify EF Core Tools installation
dotnet ef --version
```

### 2. Configure Application

Edit `appsettings.json`:
- Update the database connection string
- Change JWT SecretKey for production

### 3. Create Database

```bash
# Apply migrations to create database
dotnet ef database update
```

### 4. Run Application

```bash
# Start the application
dotnet run
```

Access Swagger UI at: `https://localhost:5001` (or the port shown in console)

---

## Common Commands

### Database Operations

```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations to database
dotnet ef database update

# Rollback to a specific migration
dotnet ef database update <MigrationName>

# Remove last migration (if not applied)
dotnet ef migrations remove

# View migration history
dotnet ef migrations list

# Drop database
dotnet ef database drop
```

### Application Commands

```bash
# Run application
dotnet run

# Run in watch mode (auto-restart on changes)
dotnet watch run

# Build application
dotnet build

# Restore packages
dotnet restore
```

---

## Troubleshooting

### ❌ Error: `update-database` not recognized

**Problem:** You're using the wrong command syntax (EF6 instead of EF Core)

**Solution:**
```bash
# ❌ WRONG (EF6 command)
update-database

# ✅ CORRECT (EF Core command)
dotnet ef database update
```

### ❌ Error: `dotnet-ef` not found

**Problem:** EF Core tools not installed

**Solution:**
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Or update if already installed
dotnet tool update --global dotnet-ef
```

### ❌ Error: Cannot connect to database

**Problem:** Connection string is incorrect or SQL Server is not running

**Solution:**
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure database name and server name are correct
4. For local SQL Server Express, use: `Server=localhost\\SQLEXPRESS;...`

### ❌ Error: Migration already applied

**Problem:** Trying to reapply a migration

**Solution:**
```bash
# Check current migration status
dotnet ef migrations list

# If you need to revert, rollback first
dotnet ef database update <PreviousMigrationName>

# Then apply the migration again
dotnet ef database update
```

---

## Quick Reference

| Task | Command |
|------|---------|
| Install EF Tools | `dotnet tool install --global dotnet-ef` |
| Update EF Tools | `dotnet tool update --global dotnet-ef` |
| Check EF Version | `dotnet ef --version` |
| Create Migration | `dotnet ef migrations add <Name>` |
| Apply Migrations | `dotnet ef database update` |
| Run Application | `dotnet run` |
| Watch Mode | `dotnet watch run` |
| Build Project | `dotnet build` |
| Restore Packages | `dotnet restore` |

---

## Visual Studio Users

If you're using Visual Studio 2022:

### Package Manager Console

You can still use Package Manager Console, but with EF Core commands:

```powershell
# ✅ Use this in Package Manager Console:
dotnet ef database update

# ❌ NOT this (EF6 command):
Update-Database
```

### Alternative: Use Terminal

In Visual Studio, open **Terminal** (View → Terminal) and use the regular `dotnet ef` commands.

---

## Environment-Specific Configuration

### Development
- Uses `appsettings.Development.json`
- Connection string: Local SQL Server
- JWT: Development secret key (change for production!)

### Production
- Create `appsettings.Production.json`
- Use secure connection strings (Azure Key Vault recommended)
- Use strong JWT secret key (min 32 characters)
- Enable HTTPS only
- Update CORS policy for your domain

---

## Need Help?

1. Check the main [README.md](README.md) for detailed documentation
2. Review the [Troubleshooting section](#troubleshooting) above
3. Verify all prerequisites are installed
4. Check that SQL Server is running
5. Ensure EF Core tools are installed globally

---

## Summary

**Key Point:** This project uses **Entity Framework Core** (not EF6), so you must use `dotnet ef` commands, not `Update-Database`.

**Essential Setup:**
1. Install `dotnet-ef` tools globally
2. Update connection string in `appsettings.json`
3. Run `dotnet ef database update`
4. Run `dotnet run`

That's it! 🚀
