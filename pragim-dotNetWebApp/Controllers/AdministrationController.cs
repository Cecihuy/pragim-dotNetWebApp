using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace pragim_dotNetWebApp.Controllers {
  public class AdministrationController : Controller {
    private readonly RoleManager<IdentityRole> roleManager;

    public AdministrationController(RoleManager<IdentityRole> roleManager) {
      this.roleManager=roleManager;
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
  }
}
