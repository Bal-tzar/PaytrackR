using Microsoft.EntityFrameworkCore;

namespace PaytrackR.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add your DbSets here, for example:
    // public DbSet<Document> Documents { get; set; }
}