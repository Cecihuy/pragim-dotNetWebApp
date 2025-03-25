using Microsoft.EntityFrameworkCore;

namespace pragim_dotNetWebApp.Models {
  public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }
    public DbSet<Employee> Employees { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.SeedEmployee();
    }
  }
}
