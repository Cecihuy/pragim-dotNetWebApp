using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace pragim_dotNetWebApp.Controllers {
  public class ErrorController : Controller {
    [Route("error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode) {
      IStatusCodeReExecuteFeature? statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
      switch(statusCode) {
        case 404:
          ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
          ViewBag.Path = statusCodeReExecuteFeature.OriginalPath;
          ViewBag.QueryString = statusCodeReExecuteFeature.OriginalQueryString;
          break;
      }
      return View("notFound");
    }
    [Route("error")]
    [AllowAnonymous]
    public IActionResult Error() {
      IExceptionHandlerFeature? exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
      ViewBag.ExceptionPath = exceptionHandlerFeature.Path;
      ViewBag.ExceptionMessage = exceptionHandlerFeature.Error.Message;
      ViewBag.StackTrace = exceptionHandlerFeature.Error.StackTrace;
      return View("error");
    }
  }
}
