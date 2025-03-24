using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;
using System.Collections.Generic;

namespace pragim_dotNetWebApp.Controllers {  
  public class HomeController : Controller {
    private readonly IEmployeeRepository _employeeRepository;
    public HomeController(IEmployeeRepository employeeRepository) {
      _employeeRepository = employeeRepository;
    }
    public ViewResult Index() {
      IEnumerable<Employee> model = _employeeRepository.GetAllEmployee();
      return View(model);
    }
    public ViewResult Details(int? id) {
      HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel {
        Employee = _employeeRepository.GetEmployee(id??1),
        PageTitle = "Employee Details"
      };
      return View(homeDetailsViewModel);
    }
    public ViewResult Create() {
      return View();
    }
  }
}
