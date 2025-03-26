using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace pragim_dotNetWebApp.Controllers {
  public class ErrorController : Controller {
    private readonly ILogger<ErrorController> logger;

    public ErrorController(ILogger<ErrorController> logger) {
      this.logger=logger;
    }
    [Route("error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode) {
      IStatusCodeReExecuteFeature? statusCodeReExecuteFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
      switch(statusCode) {
        case 404:
          ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
          ViewBag.Path = statusCodeReExecuteFeature.OriginalPath;
          logger.LogWarning(
            $"404 error occured. Path = {statusCodeReExecuteFeature.OriginalPath}" +
            $"and Query String = {statusCodeReExecuteFeature.OriginalQueryString}"
          );
          break;
      }
      return View("notFound");
    }
    [Route("error")]
    [AllowAnonymous]
    public IActionResult Error() {
      IExceptionHandlerFeature? exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
      logger.LogError($"The path {exceptionHandlerFeature.Path} threw an exception {exceptionHandlerFeature.Error}");
      return View("error");
    }
  }
}
