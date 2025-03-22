using Microsoft.AspNetCore.Mvc;

namespace pragim_dotNetWebApp.Controllers {
  public class HomeController : Controller {
    public JsonResult Index() {
      return Json(new { id = 1, name = "pragim" });
    }
  }
}
