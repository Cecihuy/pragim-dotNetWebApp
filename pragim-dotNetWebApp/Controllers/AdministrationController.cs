using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pragim_dotNetWebApp.Controllers {
  public class AdministrationController : Controller {
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;

    public AdministrationController(
      RoleManager<IdentityRole> roleManager, 
      UserManager<ApplicationUser> userManager
    ) {
      this.roleManager=roleManager;
      this.userManager=userManager;
    }
    [HttpGet]
    public IActionResult CreateRole() {
      return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model) {
      if(ModelState.IsValid) {
        IdentityResult identityResult = await roleManager.CreateAsync(
          new IdentityRole { Name = model.RoleName}
        );
        if(identityResult.Succeeded) {
          return RedirectToAction("listRoles", "administration");
        }
        foreach(IdentityError error in identityResult.Errors) {
          ModelState.AddModelError("", $"{error.Code} ==> {error.Description}");
        }
      }
      return View(model);
    }
    [HttpGet]
    public IActionResult ListRoles() {
      IQueryable<IdentityRole> roles = roleManager.Roles;
      return View(roles);
    }
    [HttpGet]
    public async Task<IActionResult> EditRole(string id) {
      IdentityRole? identityRole = await roleManager.FindByIdAsync(id);
      if(identityRole == null) {
        ViewBag.ErrorMessage = $"Role with Id = {id} can not be found";
        return View("notFound");
      }
      EditRoleViewModel editRoleViewModel = new EditRoleViewModel() {
        Id = identityRole.Id,
        RoleName = identityRole.Name
      };
      List<ApplicationUser> userList = new List<ApplicationUser>();
      UserManager<ApplicationUser> userManagerList = userManager;
      foreach(ApplicationUser user in userManagerList.Users) {
        userList.Add(user);
      }
      UserManager<ApplicationUser> userManagerLoop = userManager;
      foreach(ApplicationUser user in userList) {
        // need to create two variables of userManager because,
        // IsInRoleAsync() need datareader of existing userManager object to be close first
        if(await userManagerLoop.IsInRoleAsync(user, identityRole.Name)) {
          editRoleViewModel.Users.Add(user.UserName);
        }
      }
      return View(editRoleViewModel);
    }
    [HttpPost]
    public async Task<IActionResult> EditRole(EditRoleViewModel model) {
      IdentityRole? identityRole = await roleManager.FindByIdAsync(model.Id);
      if(identityRole == null) {
        ViewBag.ErrorMessage = $"Role with Id = {model.Id} can not be found";
        return View("notFound");
      } else {
        identityRole.Name = model.RoleName;
        IdentityResult identityResult = await roleManager.UpdateAsync(identityRole);
        if(identityResult.Succeeded) {
          return RedirectToAction("listRoles");
        }
        foreach(IdentityError error in identityResult.Errors) {
          ModelState.AddModelError("", $"{error.Code} ==> {error.Description}");
        }
        return View(model);
      }
    }
  }
}
