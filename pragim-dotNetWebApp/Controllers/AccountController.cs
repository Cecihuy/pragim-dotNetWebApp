using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace pragim_dotNetWebApp.Controllers {
  public class AccountController : Controller {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AccountController(
      UserManager<ApplicationUser> userManager, 
      SignInManager<ApplicationUser> signInManager
    ) {
      this.userManager=userManager;
      this.signInManager=signInManager;
    }
    [HttpGet][AllowAnonymous]
    public IActionResult Register() {
      return View();
    }
    [HttpPost][HttpGet][AllowAnonymous]
    public async Task<IActionResult> IsEmailInUse(string email) {
      ApplicationUser? identityUser = await userManager.FindByEmailAsync(email);
      if(identityUser == null) {
        return Json(true);
      } else {
        return Json($"Email {email} is already in use");
      }
    }
    [HttpPost][AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model) {
      if(ModelState.IsValid) {
        ApplicationUser identityUser = new ApplicationUser() { 
          UserName = model.Email, Email = model.Email, City = model.City
        };
        IdentityResult identityResult = await userManager.CreateAsync(identityUser, model.Password);
        if(identityResult.Succeeded) {
          if(signInManager.IsSignedIn(User) && User.IsInRole("Admin")) {
            return RedirectToAction("listUsers", "administration");
          }
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
    [HttpGet][AllowAnonymous]
    public IActionResult Login() {
      return View("login");
    }
    [HttpPost][AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl) {
      if(ModelState.IsValid) {
        SignInResult signInResult = await signInManager.PasswordSignInAsync(
          model.Email, model.Password, model.RememberMe, false
        );
        if(signInResult.Succeeded) {
          if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
            return Redirect(returnUrl); 
          } else { 
            return RedirectToAction("index", "home"); 
          }
        }
        ModelState.AddModelError("", "Invalid Login Attempt");
      }
      return View(model);
    }    
  }
}
