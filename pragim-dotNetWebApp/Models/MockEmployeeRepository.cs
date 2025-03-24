using System.Collections.Generic;
using System.Linq;

namespace pragim_dotNetWebApp.Models {
  public class MockEmployeeRepository : IEmployeeRepository {
    private List<Employee> _employees;
    public MockEmployeeRepository() {
      _employees = new List<Employee>() { 
        new Employee() { Id= 1, Name= "Mary", Department= Dept.HR, Email= "mary@pragimtech.com"},
        new Employee() { Id= 2, Name= "John", Department= Dept.IT, Email= "john@pragimtech.com"},
        new Employee() { Id= 3, Name= "Sam", Department= Dept.IT, Email= "sam@pragimtech.com"}
      };
    }
    public IEnumerable<Employee> GetAllEmployee() {
      return _employees;
    }
    public Employee GetEmployee(int id) {
      return _employees.FirstOrDefault(e => e.Id == id);
    }
    public Employee AddEmployee(Employee employee) {
      employee.Id = _employees.Max(empId => empId.Id) + 1;
      _employees.Add(employee);
      return employee;
    }
    public Employee UpdateEmployee(Employee employee) {
      Employee? data = _employees.FirstOrDefault(e => e.Id == employee.Id);
      if(data != null) {
        data.Id = employee.Id;
        data.Name = employee.Name;
        data.Email = employee.Email;
        data.Department = employee.Department;
      }
      return employee;
    }
    public Employee DeleteEmployee(int id) {
      Employee? employee = _employees.FirstOrDefault(e => e.Id == id);
      if(employee != null) { _employees.Remove(employee); }      
      return employee;
    }
  }
}
