using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;

namespace pragim_dotNetWebApp.Controllers {
  public class HomeController : Controller {
    private IEmployeeRepository _employeeRepository;
    public HomeController(IEmployeeRepository employeeRepository) {
      _employeeRepository = employeeRepository;
    }
    public string Index() {
      return _employeeRepository.GetEmployee(1).Name;
    }
  }
}
