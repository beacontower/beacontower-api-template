using Microsoft.EntityFrameworkCore;

namespace MyApi.Infrastructure.Data;

/// <summary>
/// Main application database context.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add DbSet properties here
    // Example: public DbSet<MyEntity> MyEntities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity mappings here
        // Example: modelBuilder.ApplyConfiguration(new MyEntityConfiguration());
    }
}
