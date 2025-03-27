using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace pragim_dotNetWebApp.Controllers {
  public class AccountController : Controller {
    private readonly UserManager<IdentityUser> userManager;
    private readonly SignInManager<IdentityUser> signInManager;

    public AccountController(
      UserManager<IdentityUser> userManager, 
      SignInManager<IdentityUser> signInManager
    ) {
      this.userManager=userManager;
      this.signInManager=signInManager;
    }
    [HttpGet]
    public IActionResult Register() {
      return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model) {
      if(ModelState.IsValid) {
        IdentityUser identityUser = new IdentityUser() { UserName = model.Email, Email = model.Email };
        IdentityResult identityResult = await userManager.CreateAsync(identityUser, model.Password);
        if(identityResult.Succeeded) {
          await signInManager.SignInAsync(identityUser, false);
          return RedirectToAction("index", "home");
        }
        foreach(IdentityError error in identityResult.Errors) {
          ModelState.AddModelError("", $"{error.Code}, {error.Description}");
        }
      }
      return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Logout() {
      await signInManager.SignOutAsync();
      return RedirectToAction("index", "home");
    }
  }
}
