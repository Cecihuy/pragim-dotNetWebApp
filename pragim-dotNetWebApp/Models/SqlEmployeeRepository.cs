using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace pragim_dotNetWebApp.Models {
  public class SqlEmployeeRepository : IEmployeeRepository {
    private readonly AppDbContext appDbContext;
    private readonly ILogger<SqlEmployeeRepository> logger;

    public SqlEmployeeRepository(AppDbContext appDbContext, ILogger<SqlEmployeeRepository> logger) {
      this.appDbContext=appDbContext;
      this.logger=logger;
    }
    public IEnumerable<Employee> GetAllEmployee() {
      return appDbContext.Employees;
    }
    public Employee GetEmployee(int id) {
      logger.LogTrace("Trace Log");
      logger.LogDebug("Debug Log");
      logger.LogInformation("Information Log");
      logger.LogWarning("Warning Log");
      logger.LogError("Error Log");
      logger.LogCritical("Critical Log");
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
