using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;
using System.Linq;
using System.Security.Claims;
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
    public async Task<IActionResult> Login(string? returnUrl) {
      LoginViewModel loginViewModel = new LoginViewModel() {
        ReturnUrl = returnUrl,
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
      };
      return View(loginViewModel);
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
    [AllowAnonymous]
    [HttpPost]
    public IActionResult ExternalLogin(string provider, string returnUrl) {
      string? redirectUrl = 
        Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
      AuthenticationProperties authenticationProperties = 
        signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
      return new ChallengeResult(provider, authenticationProperties);
    }
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(
      string returnUrl = null,
      string remoteError = null
    ) {
      returnUrl = returnUrl ?? Url.Content("~/");
      LoginViewModel loginViewModel = new LoginViewModel() {
        ReturnUrl = returnUrl,
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
      };
      if(remoteError != null) {
        ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
        return View("Login", loginViewModel);
      }
      ExternalLoginInfo? externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
      if(externalLoginInfo == null) {
        ModelState.AddModelError(string.Empty, "Error loading external login information");
        return View("Login", loginViewModel);
      }
      SignInResult signInResult = await signInManager.ExternalLoginSignInAsync(
        externalLoginInfo.LoginProvider, 
        externalLoginInfo.ProviderKey, 
        false, true
      );
      if(signInResult.Succeeded) {
        return LocalRedirect(returnUrl);
      } else {
        string? emailClaim = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
        if(emailClaim != null) {
          ApplicationUser? applicationUser = await userManager.FindByEmailAsync(emailClaim);
          if(applicationUser == null) {
            applicationUser = new ApplicationUser () {
              UserName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
              Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)
            };
            await userManager.CreateAsync(applicationUser);
          }
          await userManager.AddLoginAsync(applicationUser, externalLoginInfo);
          await signInManager.SignInAsync(applicationUser, false);
          return LocalRedirect(returnUrl);
        }
        ViewBag.ErrorTitle = $"Email claim not received from: {externalLoginInfo.LoginProvider}";
        ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";
        return View("error");
      }
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() {
      return View();
    }
  }
}
