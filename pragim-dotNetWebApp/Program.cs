using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Models;
using Microsoft.AspNetCore.Routing;

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
      app.UseMvc(config => {
        config.MapRoute("custom", "{controller=Home}/{action=Index}/{id?}");
      });
      app.Run();
    }
  }
}
