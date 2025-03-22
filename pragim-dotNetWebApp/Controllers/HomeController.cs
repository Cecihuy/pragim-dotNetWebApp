﻿using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;

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
      Employee model = _employeeRepository.GetEmployee(1);
      return View(model);
    }
  }
}
