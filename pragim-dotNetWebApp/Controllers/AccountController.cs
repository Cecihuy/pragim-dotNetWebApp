using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using pragim_dotNetWebApp.Models;
using pragim_dotNetWebApp.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace pragim_dotNetWebApp.Controllers {
  [Authorize(Policy = "ControllerRolePolicy")]
  public class AccountController : Controller {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<AccountController> logger;

    public AccountController(
      UserManager<ApplicationUser> userManager, 
      SignInManager<ApplicationUser> signInManager,
      ILogger<AccountController> logger
    ) {
      this.userManager=userManager;
      this.signInManager=signInManager;
      this.logger=logger;
    }
    [HttpGet]
    public async Task<IActionResult> AddPassword() {
      ApplicationUser? applicationUser = await userManager.GetUserAsync(User);
      bool userHasPassword = await userManager.HasPasswordAsync(applicationUser);
      if(userHasPassword) {
        return RedirectToAction("ChangePassword");
      }
      return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddPassword(AddPasswordViewModel model) {
      if(ModelState.IsValid) {
        ApplicationUser? applicationUser = await userManager.GetUserAsync(User);
        IdentityResult identityResult = await userManager.AddPasswordAsync(applicationUser, model.NewPassword);
        if(!identityResult.Succeeded) {
          foreach(IdentityError error in identityResult.Errors) {
            ModelState.AddModelError(string.Empty, $"{error.Code} ==> {error.Description}");
          }
          return View();
        }
        await signInManager.RefreshSignInAsync(applicationUser);
        return View("AddPasswordConfirmation");
      }
      return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> ChangePassword() {
      ApplicationUser? applicationUser = await userManager.GetUserAsync(User);
      bool userHasPassword = await userManager.HasPasswordAsync(applicationUser);
      if(!userHasPassword) {
        return RedirectToAction("AddPassword");
      }
      return View();
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
      if(ModelState.IsValid) {
        ApplicationUser? applicationUser = await userManager.GetUserAsync(User);
        if(applicationUser == null) {
          return RedirectToAction("Login");
        }
        IdentityResult identityResult = await userManager.ChangePasswordAsync(
          applicationUser, model.CurrentPassword, model.NewPassword
        );
        if(!identityResult.Succeeded) {
          foreach(IdentityError error in identityResult.Errors) {
            ModelState.AddModelError(string.Empty, $"{error.Code} ==> {error.Description}");
          }
          return View();
        }
        await signInManager.RefreshSignInAsync(applicationUser);
        return View("ChangePasswordConfirmation");
      }
      return View(model);
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
          string token = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
          string? confirmationLink = Url.Action(
            "ConfirmEmail",
            "Account", 
            new { UserId = identityUser.Id, Token = token }, 
            Request.Scheme
          );
          logger.Log(LogLevel.Warning, confirmationLink);
          if(signInManager.IsSignedIn(User) && User.IsInRole("Admin")) {
            return RedirectToAction("listUsers", "administration");
          }
          ViewBag.ErrorTitle = "Registration Successfull";
          ViewBag.ErrorMessage = "Before you can login, please confirm your " +
            "email, by clicking on the confirmation link we have emailed you";
          return View("Error");
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
      model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
      if(ModelState.IsValid) {
        ApplicationUser? applicationUser = await userManager.FindByEmailAsync(model.Email);
        if(
          applicationUser != null && 
          !applicationUser.EmailConfirmed &&
          (await userManager.CheckPasswordAsync(applicationUser, model.Password))
        ) {
          ModelState.AddModelError(string.Empty, "Email not confirmed yet");
          return View(model);
        }
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
      string? emailClaim = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
      ApplicationUser? applicationUser = null;
      if(emailClaim != null) {
        applicationUser = await userManager.FindByEmailAsync(emailClaim);
        if(applicationUser != null && !applicationUser.EmailConfirmed) {
          ModelState.AddModelError(string.Empty, "Email not confirmed yet");
          return View("Login", loginViewModel);
        }
      }
      SignInResult signInResult = await signInManager.ExternalLoginSignInAsync(
        externalLoginInfo.LoginProvider, 
        externalLoginInfo.ProviderKey, 
        false, true
      );
      if(signInResult.Succeeded) {
        return LocalRedirect(returnUrl);
      } else {
        
        if(emailClaim != null) {
          if(applicationUser == null) {
            applicationUser = new ApplicationUser () {
              UserName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
              Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email)
            };
            await userManager.CreateAsync(applicationUser);
            string token = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            string? confirmationLink = Url.Action(
              "ConfirmEmail",
              "Account",
              new { UserId = applicationUser.Id, Token = token },
              Request.Scheme
            );
            logger.Log(LogLevel.Warning, confirmationLink);
            ViewBag.ErrorTitle = "Registration Successfull";
            ViewBag.ErrorMessage = "Before you can login, please confirm your " +
              "email, by clicking on the confirmation link we have emailed you";
            return View("Error");
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
    public async Task<IActionResult> ConfirmEmail(string userId, string token) {
      if(userId == null || token == null) {
        RedirectToAction("Index", "Home");
      }
      ApplicationUser? applicationUser = await userManager.FindByIdAsync(userId);
      if(applicationUser == null) {
        ViewBag.ErrorMessage = $"The user id {userId} is invalid";
        return View("NotFound");
      }
      IdentityResult identityResult = await userManager.ConfirmEmailAsync(applicationUser, token);
      if(identityResult.Succeeded) {
        return View();
      }
      ViewBag.ErrorTitle = "Email cannot be confirmed";
      return View("Error");
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword() {
      return View();
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model) {
      if(ModelState.IsValid) {
        ApplicationUser? applicationUser = await userManager.FindByEmailAsync(model.Email);
        if(applicationUser != null && await userManager.IsEmailConfirmedAsync(applicationUser)) {
          string token = await userManager.GeneratePasswordResetTokenAsync(applicationUser);
          string? passwordResetLink = Url.Action(
            "ResetPassword", 
            "Account",
            new { Email = model.Email, Token = token }, Request.Scheme);
          logger.Log(LogLevel.Warning, passwordResetLink);
          return View("ForgotPasswordConfirmation");
        }
        return View("ForgotPasswordConfirmation");
      }
    return View(model);
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email) {
      if(token == null || email == null) {
        ModelState.AddModelError(string.Empty, "Invalid password reset token");
      }
      return View();
    }
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) {
      if(ModelState.IsValid) {
        ApplicationUser? applicationUser = await userManager.FindByEmailAsync(model.Email);
        if(applicationUser != null) {
          IdentityResult identityResult = await userManager.ResetPasswordAsync(applicationUser, model.Token, model.Password);
          if(identityResult.Succeeded) {
            return View("ResetPasswordConfirmation");
          }
          foreach(IdentityError error in identityResult.Errors) {
            ModelState.AddModelError(string.Empty, $"{error.Code} ==> {error.Description}");
          }
          return View(model);
        }
        return View("ResetPasswordConfirmation");
      } 
      return View(model);
    }  
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() {
      return View();
    }
  }
}
