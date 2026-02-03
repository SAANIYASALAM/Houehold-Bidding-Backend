using Household_Bidding.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Household_Bidding.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CustomerProfile> CustomerProfiles { get; set; }
    public DbSet<WorkerProfile> WorkerProfiles { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<ServiceCategory> ServiceCategories { get; set; }
    public DbSet<WorkerSkill> WorkerSkills { get; set; }
    public DbSet<Models.Entities.Task> Tasks { get; set; }
    public DbSet<Bid> Bids { get; set; }
    public DbSet<TaskAssignment> TaskAssignments { get; set; }
    public DbSet<TaskRevision> TaskRevisions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // CustomerProfile configuration
        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.User)
                .WithOne(u => u.CustomerProfile)
                .HasForeignKey<CustomerProfile>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.City)
                .WithMany(c => c.CustomerProfiles)
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // WorkerProfile configuration
        modelBuilder.Entity<WorkerProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.User)
                .WithOne(u => u.WorkerProfile)
                .HasForeignKey<WorkerProfile>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.City)
                .WithMany(c => c.WorkerProfiles)
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // City configuration
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // ServiceCategory configuration
        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // WorkerSkill configuration (many-to-many)
        modelBuilder.Entity<WorkerSkill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.WorkerProfileId, e.ServiceCategoryId }).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.WorkerProfile)
                .WithMany(w => w.WorkerSkills)
                .HasForeignKey(e => e.WorkerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ServiceCategory)
                .WithMany(s => s.WorkerSkills)
                .HasForeignKey(e => e.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Task configuration
        modelBuilder.Entity<Models.Entities.Task>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Budget).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.CustomerProfile)
                .WithMany(c => c.Tasks)
                .HasForeignKey(e => e.CustomerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ServiceCategory)
                .WithMany(s => s.Tasks)
                .HasForeignKey(e => e.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.City)
                .WithMany(c => c.Tasks)
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Bid configuration
        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProposedAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Task)
                .WithMany(t => t.Bids)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.WorkerProfile)
                .WithMany(w => w.Bids)
                .HasForeignKey(e => e.WorkerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskAssignment configuration with filtered unique index for one active task per worker
        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaskId).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Filtered unique index: only one active task per worker
            entity.HasIndex(e => e.WorkerProfileId)
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            entity.HasOne(e => e.Task)
                .WithOne(t => t.TaskAssignment)
                .HasForeignKey<TaskAssignment>(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.WorkerProfile)
                .WithMany(w => w.TaskAssignments)
                .HasForeignKey(e => e.WorkerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Bid)
                .WithMany()
                .HasForeignKey(e => e.BidId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskRevision configuration
        modelBuilder.Entity<TaskRevision>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Resolution).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Task)
                .WithMany(t => t.TaskRevisions)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaskId).IsUnique();
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TransactionId).HasMaxLength(200);
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Task)
                .WithOne(t => t.Payment)
                .HasForeignKey<Payment>(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CustomerProfile)
                .WithMany()
                .HasForeignKey(e => e.CustomerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.WorkerProfile)
                .WithMany()
                .HasForeignKey(e => e.WorkerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Review configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaskId).IsUnique();
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Task)
                .WithOne(t => t.Review)
                .HasForeignKey<Review>(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CustomerProfile)
                .WithMany(c => c.Reviews)
                .HasForeignKey(e => e.CustomerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.WorkerProfile)
                .WithMany(w => w.ReceivedReviews)
                .HasForeignKey(e => e.WorkerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
