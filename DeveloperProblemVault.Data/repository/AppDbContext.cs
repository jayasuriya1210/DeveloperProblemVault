using Microsoft.EntityFrameworkCore;

namespace DeveloperProblemVault.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Issue> Issues { get; set; }
}
