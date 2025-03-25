using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace pragim_dotNetWebApp.Controllers {  
  public class HomeController : Controller {
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IHostEnvironment hostEnvironment;

    public HomeController(IEmployeeRepository employeeRepository, IHostEnvironment hostEnvironment) {
      _employeeRepository = employeeRepository;
      this.hostEnvironment=hostEnvironment;
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
    [HttpGet]
    public ViewResult Create() {
      return View();
    }
    [HttpPost]
    public IActionResult Create(EmployeeCreateViewModel model) {
      if(ModelState.IsValid) {
        string uniqueFileName = ProcessUploadedFile(model);
        Employee newEmployee = new Employee {
          Name = model.Name,
          Email = model.Email,
          Department = model.Department,
          PhotoPath = uniqueFileName
        };
        _employeeRepository.AddEmployee(newEmployee);
        return RedirectToAction("details", new { id = newEmployee.Id });
      }
      return View();
    }
    [HttpGet]
    public ViewResult Edit(int id) {
      Employee employee = _employeeRepository.GetEmployee(id);
      EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel() {
        Id = employee.Id,
        Name = employee.Name,
        Email = employee.Email,
        Department = employee.Department,
        ExistingPhotoPath = employee.PhotoPath
      };      
      return View(employeeEditViewModel);
    }
    [HttpPost]
    public IActionResult Edit(EmployeeEditViewModel model) {
      if(ModelState.IsValid) {
        Employee employee = _employeeRepository.GetEmployee(model.Id);
        employee.Name = model.Name;
        employee.Email = model.Email;
        employee.Department = model.Department;
        if(model.Photo != null) {
          if(model.ExistingPhotoPath != null) {
            string filePath = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot/images", model.ExistingPhotoPath);
            System.IO.File.Delete(filePath);
          }
          employee.PhotoPath = ProcessUploadedFile(model);
        }
        _employeeRepository.UpdateEmployee(employee);
        return RedirectToAction("index");
      }
      return View();
    }
    private string ProcessUploadedFile(EmployeeCreateViewModel model) {
      string uniqueFileName = null;
      if(model.Photo !=null) {
        string uploadsFolder = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot/images");
        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using(FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
          model.Photo.CopyTo(fileStream);
        }
      }
      return uniqueFileName;
    }
  }
}
