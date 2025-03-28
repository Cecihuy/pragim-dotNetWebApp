using pragim_dotNetWebApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace pragim_dotNetWebApp {
  public class Program {
    public static void Main(string[] args) {
      /* =================================== variable =================================== */
      AuthorizationPolicy authorizationPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser().Build();
      /* =================================== services =================================== */
      var builder = WebApplication.CreateBuilder(args);
      builder.Services.Configure<MvcOptions>(options => {
        options.EnableEndpointRouting = false;
        options.Filters.Add(new AuthorizeFilter(authorizationPolicy));
      });
      builder.Services.Configure<IdentityOptions>(options => {
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
      });
      builder.Services.AddMvc();
      builder.Services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();
      builder.Services.AddDbContextPool<AppDbContext>(options => {
        options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeApp"));
      });
      builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>();
      /* =================================== pipeline =================================== */
      var app = builder.Build();
      if(app.Environment.IsDevelopment()) { 
        app.UseDeveloperExceptionPage();
      } else {
        app.UseExceptionHandler("/error");
        app.UseStatusCodePagesWithReExecute("/error/{0}");
      }
      app.UseStaticFiles();
      app.UseAuthentication();
      app.UseMvc(config => {
        config.MapRoute("custom", "{controller=Home}/{action=Index}/{id?}");
      });
      app.Run();
    }
  }
}
