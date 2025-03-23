using System.Collections.Generic;

namespace pragim_dotNetWebApp.Models {
  public interface IEmployeeRepository {
    Employee GetEmployee(int id);
    IEnumerable<Employee> GetAllEmployee();
  }
}
