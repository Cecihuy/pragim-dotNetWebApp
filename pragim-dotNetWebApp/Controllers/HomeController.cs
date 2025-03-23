using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;

namespace pragim_dotNetWebApp.Controllers {
  public class HomeController : Controller {
    private readonly IEmployeeRepository _employeeRepository;
    public HomeController(IEmployeeRepository employeeRepository) {
      _employeeRepository = employeeRepository;
    }
    public string Index() {
      return _employeeRepository.GetEmployee(1).Name;
    }
    public ViewResult Details() {
      HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel {
        Employee = _employeeRepository.GetEmployee(1),
        PageTitle = "Employee Details"
      };
      return View(homeDetailsViewModel);
    }
  }
}
