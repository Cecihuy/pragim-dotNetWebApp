using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pragim_dotNetWebApp.Controllers {
  [Authorize(Policy = "ControllerRolePolicy")]
  public class AdministrationController : Controller {
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<AdministrationController> ilogger;

    public AdministrationController(
      RoleManager<IdentityRole> roleManager, 
      UserManager<ApplicationUser> userManager,
      ILogger<AdministrationController> ilogger
    ) {
      this.roleManager=roleManager;
      this.userManager=userManager;
      this.ilogger=ilogger;
    }
    [HttpGet]
    public async Task<IActionResult> ManageUserClaims(string userId) {
      ApplicationUser? applicationUser = await userManager.FindByIdAsync(userId);
      if(applicationUser == null) {
        ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
        return View("NotFound");
      }
      IList<Claim> claims = await userManager.GetClaimsAsync(applicationUser);
      UserClaimsViewModel model = new UserClaimsViewModel() {
        UserId = userId
      };
      foreach(Claim claim in ClaimsStore.AllClaims) {
        UserClaim userClaim = new UserClaim() {
          ClaimType = claim.Type
        };
        if(claims.Any(c => c.Type == claim.Type)) {
          userClaim.IsSelected = true;
        }
        model.Claims.Add(userClaim);
      }
      return View(model);
    }
    [HttpPost]
    [Authorize(Policy = "AdminRolePolicy")]
    public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model) {
      ApplicationUser? applicationUser = await userManager.FindByIdAsync(model.UserId);
      if(applicationUser == null) {
        ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
        return View("NotFound");
      }
      IList<Claim> claims = await userManager.GetClaimsAsync(applicationUser);
      IdentityResult identityResult = await userManager.RemoveClaimsAsync(applicationUser, claims);
      if(!identityResult.Succeeded) {
        ModelState.AddModelError("", "Cannot remove user existing claims");
        return View(model);
      }
      identityResult = await userManager.AddClaimsAsync(applicationUser, model.Claims
        .Where(c => c.IsSelected)
        .Select(c => new Claim(c.ClaimType, c.ClaimType))
      );
      if(!identityResult.Succeeded) {
        ModelState.AddModelError("", "Cannot add selected claims to user");
        return View(model);
      }
      return RedirectToAction("editUser", new { Id = model.UserId });
    }
    [HttpGet]
    public async Task<IActionResult> ManageUserRoles(string userId) {
      ViewBag.UserId = userId;
      ApplicationUser? applicationUser = await userManager.FindByIdAsync(userId);
      if(applicationUser == null) {
        ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
        return View("NotFound");
      }
      List<UserRolesViewModel> model = new List<UserRolesViewModel>();
      // correction. add ToList() instead of creating new List<>() for closing datareader :v
      foreach(IdentityRole role in roleManager.Roles.ToList()) {
        UserRolesViewModel userRolesViewModel = new UserRolesViewModel() {
          RoleName = role.Name,
          RoleId = role.Id
        };
        if(await userManager.IsInRoleAsync(applicationUser, role.Name)) {
          userRolesViewModel.IsSelected = true;
        } else {
          userRolesViewModel.IsSelected = false;
        }
        model.Add(userRolesViewModel);
      }
      return View(model);
    }
    [HttpPost]
    [Authorize(Policy = "AdminRolePolicy")]
    public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> models, string userId) {
      ApplicationUser? applicationUser = await userManager.FindByIdAsync(userId);
      if(applicationUser == null) {
        ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
        return View("NotFound");
      }
      IList<string> roles = await userManager.GetRolesAsync(applicationUser);
      IdentityResult identityResult = await userManager.RemoveFromRolesAsync(applicationUser, roles);
      if(!identityResult.Succeeded) {
        ModelState.AddModelError("", "Cannot remove user existing roles");
      }
      identityResult = await userManager.AddToRolesAsync(applicationUser, models
        .Where(role => role.IsSelected)
        .Select(role => role.RoleName)
      );
      if(!identityResult.Succeeded) {
        ModelState.AddModelError("", "Cannot add selected roles to user");
      }
      return RedirectToAction("editUser", new { Id = userId });
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
    [Authorize(Policy = "AdminRolePolicy")]
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
    [Authorize(Policy = "AdminRolePolicy")]
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
      foreach(ApplicationUser user in userManager.Users.ToList()) {
        if(await userManager.IsInRoleAsync(user, identityRole.Name)) {
          editRoleViewModel.Users.Add(user.UserName);
        }
      }
      return View(editRoleViewModel);
    }
    [HttpPost]
    [Authorize(Policy = "AdminRolePolicy")]
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
      List<UserRoleViewModel> userRoleViewModels = new List<UserRoleViewModel>();
        foreach(ApplicationUser user in userManager.Users.ToList()) {
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
    [Authorize(Policy = "AdminRolePolicy")]
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
    [HttpPost]
    [Authorize(Policy = "DeleteClaimPolicy")]
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
    }
    [HttpPost]
    [Authorize(Policy = "DeleteClaimPolicy")]
    public async Task<IActionResult> DeleteRole(string id) {
      IdentityRole? identityRole = await roleManager.FindByIdAsync(id);
      if(identityRole == null) {
        ViewBag.ErrorMessage = $"Role with Id = {id} can not be found";
        return View("notFound");
      } else {
        try {
          IdentityResult identityResult = await roleManager.DeleteAsync(identityRole);
          if(identityResult.Succeeded) {
            return RedirectToAction("listRoles");
          }
          foreach(IdentityError error in identityResult.Errors) {
            ModelState.AddModelError("", $"{error.Code} ==> {error.Description}");
          }
          return View("listRoles");
        } catch (DbUpdateException ex) {
          ilogger.LogError($"Error deleting role : {ex}");
          ViewBag.ErrorTitle = $"{identityRole.Name} role is in use";
          ViewBag.ErrorMessage = $"{identityRole.Name} role cannot be deleted as there are users in this role. " +
            $"If you want to delete this role, please remove the users from the role and then try to delete";
          return View("error");
        }
      }
    }
  }
}
