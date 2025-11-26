using Microsoft.EntityFrameworkCore;
using PaytrackR.Models;

namespace PaytrackR.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Worker> Workers { get; set; }
    public DbSet<Shift> Shifts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Worker entity
        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(10,2)");
        });

        // Configure Shift entity
        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HourlyRateAtTimeOfShift).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TotalEarnings).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            // Relationship
            entity.HasOne(e => e.Worker)
                .WithMany(w => w.Shifts)
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}