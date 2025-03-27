using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.ViewModels;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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
    [AllowAnonymous]
    public IActionResult Register() {
      return View();
    }
    [HttpPost]
    [AllowAnonymous]
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
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() {
      return View("login");
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl) {
      if(ModelState.IsValid) {
        SignInResult signInResult = await signInManager.PasswordSignInAsync(
          model.Email, model.Password, model.RememberMe, false
        );
        if(signInResult.Succeeded) {
          if(!string.IsNullOrEmpty(returnUrl)) {
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
