using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace pragim_dotNetWebApp.Models {
  public class AppDbContext : IdentityDbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }
    public DbSet<Employee> Employees { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      base.OnModelCreating(modelBuilder);
      modelBuilder.SeedEmployee();
    }
  }
}
