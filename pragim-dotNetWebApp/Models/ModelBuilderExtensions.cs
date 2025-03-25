using Microsoft.EntityFrameworkCore;

namespace pragim_dotNetWebApp.Models {
  public static class ModelBuilderExtensions {
    public static void SeedEmployee(this ModelBuilder modelBuilder) {
      modelBuilder.Entity<Employee>()
        .HasData(
          new Employee() {
            Id= 1,
            Name= "Mary",
            Department= Dept.IT,
            Email= "mary@pragimtech.com"
          },
          new Employee() {
            Id= 2,
            Name= "John",
            Department= Dept.HR,
            Email= "john@pragimtech.com"
          }
        );
    }
  }
}
