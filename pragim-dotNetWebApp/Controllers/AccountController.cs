using Microsoft.AspNetCore.Mvc;

namespace pragim_dotNetWebApp.Controllers {
  public class AccountController : Controller {
    [HttpGet]
    public IActionResult Register() {
      return View();
    }
  }
}
