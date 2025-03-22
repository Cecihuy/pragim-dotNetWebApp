using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace pragim_dotNetWebApp {
  public class Program {
    public static void Main(string[] args) {
      /* =================================== variable =================================== */
      /* =================================== services =================================== */
      var builder = WebApplication.CreateBuilder(args);
      /* =================================== pipeline =================================== */
      var app = builder.Build();
      if(app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
      app.UseStaticFiles();
      app.Use(async (context, next) => {
        await context.Response.WriteAsync("Hosting Environtment: " + app.Environment.EnvironmentName);
        await next();
      });      
      app.Run();
    }
  }
}
