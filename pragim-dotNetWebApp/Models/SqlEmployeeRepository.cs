using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace pragim_dotNetWebApp.Models {
  public class SqlEmployeeRepository : IEmployeeRepository {
    private readonly AppDbContext appDbContext;
    public SqlEmployeeRepository(AppDbContext appDbContext) {
      this.appDbContext=appDbContext;
    }
    public IEnumerable<Employee> GetAllEmployee() {
      return appDbContext.Employees;
    }
    public Employee GetEmployee(int id) {
      return appDbContext.Employees.Find(id);
    }
    public Employee AddEmployee(Employee employee) {
      appDbContext.Employees.Add(employee);
      appDbContext.SaveChanges();
      return employee;
    }
    public Employee UpdateEmployee(Employee employee) {
      EntityEntry<Employee> entityEntry = appDbContext.Employees.Attach(employee);
      entityEntry.State = EntityState.Modified;
      appDbContext.SaveChanges();
      return employee;
    }
    public Employee DeleteEmployee(int id) {
      Employee? employee = appDbContext.Employees.Find(id);
      if(employee != null) { 
        appDbContext.Employees.Remove(employee); 
        appDbContext.SaveChanges(); 
      }
      return employee;
    }
  }
}
