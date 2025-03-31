﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pragim_dotNetWebApp.Controllers {
  [Authorize(Roles = "Admin , User")]
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
    public IActionResult ListUsers() {
      IQueryable<ApplicationUser> users = userManager.Users;
      return View(users);
    }
    [HttpGet]
    public async Task<IActionResult> EditUser(string id) {
      ApplicationUser? applicationUser = await userManager.FindByIdAsync(id);
      if(applicationUser == null) {
        ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
        return View("NotFound");
      }
      IList<Claim> claims = await userManager.GetClaimsAsync(applicationUser);
      IList<string> roles = await userManager.GetRolesAsync(applicationUser);

      EditUserViewModel editUserViewModel = new EditUserViewModel() {
        Id = applicationUser.Id,
        Email = applicationUser.Email,
        UserName = applicationUser.UserName,
        City = applicationUser.City,
        Claims = claims.Select(claim => claim.Value).ToList(),
        Roles = roles
      };
      return View(editUserViewModel);
    }
    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserViewModel model) {
      ApplicationUser? applicationUser = await userManager.FindByIdAsync(model.Id);
      if(applicationUser == null) {
        ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
        return View("NotFound");
      } else {
        applicationUser.Email = model.Email;
        applicationUser.UserName = model.UserName;
        applicationUser.City = model.City;
        IdentityResult identityResult = await userManager.UpdateAsync(applicationUser);
        if(identityResult.Succeeded) {
          return RedirectToAction("listUsers");
        }
        foreach(IdentityError error in identityResult.Errors)
          ModelState.AddModelError("", $"{error.Code} ==> {error.Description}");
        }
      return View(model);
    }
    [HttpGet]
    public IActionResult CreateRole() {
      return View();
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
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
      foreach(ApplicationUser user in userManager.Users) {
        userList.Add(user);
      }
      // correction: datareader automatically close after finishing foreach scope
      // no need to create variables for userManager
      foreach(ApplicationUser user in userList) {
        if(await userManager.IsInRoleAsync(user, identityRole.Name)) {
          editRoleViewModel.Users.Add(user.UserName);
        }
      }
      return View(editRoleViewModel);
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
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
    [HttpGet]
    public async Task<IActionResult> EditUsersInRole(string roleId) {
      ViewBag.RoleId = roleId;
      IdentityRole? identityRole = await roleManager.FindByIdAsync(roleId);
      if(identityRole == null) {
        ViewBag.ErrorMessage = $"Role with Id = {roleId} can not be found";
        return View("notFound");
      }
      List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
      foreach(ApplicationUser user in userManager.Users) {        
        applicationUsers.Add(user);
      }
      List<UserRoleViewModel> userRoleViewModels = new List<UserRoleViewModel>();
        foreach(ApplicationUser user in applicationUsers) {
          UserRoleViewModel userRoleViewModel = new UserRoleViewModel() {
            UserId = user.Id,
            UserName = user.UserName
          };
          if(await userManager.IsInRoleAsync(user, identityRole.Name)) {
            userRoleViewModel.IsSelected = true;
          } else {
            userRoleViewModel.IsSelected = false;
          }
          userRoleViewModels.Add(userRoleViewModel);
        }
      return View(userRoleViewModels);
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId) {
      IdentityRole? identityRole = await roleManager.FindByIdAsync(roleId);
      if(identityRole == null) {
        ViewBag.ErrorMessage = $"Role with Id = {roleId} can not be found";
        return View("notFound");
      }
      for(int i=0; i<model.Count; i++) {
        ApplicationUser? applicationUser = await userManager.FindByIdAsync(model[i].UserId);
        IdentityResult identityResult = null;
        if(model[i].IsSelected && !(await userManager.IsInRoleAsync(applicationUser,identityRole.Name))) {
          identityResult = await userManager.AddToRoleAsync(applicationUser, identityRole.Name);
        } else if (!(model[i].IsSelected) && await userManager.IsInRoleAsync(applicationUser, identityRole.Name)) {
          identityResult = await userManager.RemoveFromRoleAsync(applicationUser, identityRole.Name);
        } else { 
          continue; 
        }
        if(identityResult.Succeeded) {
          if(i < model.Count - 1) {
            continue;
          } else {
            return RedirectToAction("editRole", new { id = roleId });
          }
        }
      }
      return RedirectToAction("editRole", new { id = roleId });
    }
    public async Task<IActionResult> DeleteUser(string id) {
      ApplicationUser? applicationUser = await userManager.FindByIdAsync(id);
      if(applicationUser == null) {
        ViewBag.ErrorMessage = $"User with Id = {id} can not be found";
        return View("notFound");
      } else {
        IdentityResult identityResult = await userManager.DeleteAsync(applicationUser);
        if(identityResult.Succeeded) {
          return RedirectToAction("listUsers");
        }
        foreach(IdentityError error in identityResult.Errors) {
          ModelState.AddModelError("", $"{error.Code} ==> {error.Description}");
        }
        return View("listUsers");
      }

      return View();
    }
  }
}
