using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;

namespace pragim_dotNetWebApp {
  public class Program {
    public static void Main(string[] args) {
      /* =================================== variable =================================== */
      
      /* =================================== services =================================== */
      var builder = WebApplication.CreateBuilder(args);
      builder.Services.Configure<MvcOptions>(options => {
        options.EnableEndpointRouting = false;
      });
      builder.Services.AddMvc();
      builder.Services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
      /* =================================== pipeline =================================== */
      var app = builder.Build();
      if(app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
      app.UseMvcWithDefaultRoute();
      app.Use(async (context, next) => {
        await context.Response.WriteAsync("Hello World");
        await next();
      });
      app.Run();
    }
  }
}
