using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace pragim_dotNetWebApp {
  public class Program {
    public static void Main(string[] args) {
      /* =================================== services =================================== */
      var builder = WebApplication.CreateBuilder(args);
      /* =================================== pipeline =================================== */
      var app = builder.Build();
      app.Use(async (context, next) => {
        app.Logger.LogInformation("MW1: Incoming Request");
        await next();
        app.Logger.LogInformation("MW1: Outgoing Response");
      });
      app.Use(async (context, next) => {
        app.Logger.LogInformation("MW2: Incoming Request");
        await next();
        app.Logger.LogInformation("MW2: Outgoing Response");
      });
      app.MapGet("/", async (context) => {
        await context.Response.WriteAsync("MW3: Request handled and response produced");
        app.Logger.LogInformation("MW3: Request handled and response produced");
      });
      app.Run();
    }
  }
}
