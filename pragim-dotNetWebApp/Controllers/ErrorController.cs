using Microsoft.AspNetCore.Mvc;

namespace pragim_dotNetWebApp.Controllers {
  public class ErrorController : Controller {
    [Route("error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode) {
      switch(statusCode) {
        case 404:
          ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
          break;
      }
      return View("notFound");
    }
  }
}
