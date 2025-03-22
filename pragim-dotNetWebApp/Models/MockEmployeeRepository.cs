using System.Collections.Generic;
using System.Linq;

namespace pragim_dotNetWebApp.Models {
  public class MockEmployeeRepository : IEmployeeRepository {
    private List<Employee> _employees;
    public MockEmployeeRepository() {
      _employees = new List<Employee>() { 
        new Employee() { Id= 1, Name= "Mary", Department= "HR", Email= "mary@pragimtech.com"},
        new Employee() { Id= 2, Name= "John", Department= "IT", Email= "john@pragimtech.com"},
        new Employee() { Id= 3, Name= "Sam", Department= "IT", Email= "sam@pragimtech.com"}
      };
    }
    public Employee GetEmployee(int id) {
      return _employees.FirstOrDefault(e => e.Id == id);
    }
  }
}
