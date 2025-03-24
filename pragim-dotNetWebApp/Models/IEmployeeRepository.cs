using System.Collections.Generic;

namespace pragim_dotNetWebApp.Models {
  public interface IEmployeeRepository {
    Employee GetEmployee(int id);
    IEnumerable<Employee> GetAllEmployee();
    Employee AddEmployee(Employee employee);
    Employee UpdateEmployee(Employee employee);
    Employee DeleteEmployee(int id);
  }
}
