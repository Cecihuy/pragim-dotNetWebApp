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
using pragim_dotNetWebApp.Security;
using System;

namespace pragim_dotNetWebApp {
  public class Program {
    public static void Main(string[] args) {
      /* =================================== variable =================================== */
      
      /* =================================== services =================================== */
      var builder = WebApplication.CreateBuilder(args);
      builder.Services.Configure<MvcOptions>(options => {
        options.EnableEndpointRouting = false;
      });
      builder.Services.Configure<IdentityOptions>(options => {
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.SignIn.RequireConfirmedEmail = true;
        options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
      });
      builder.Services.Configure<DataProtectionTokenProviderOptions>(options => {
        options.TokenLifespan = TimeSpan.FromMinutes(1);
      });
      builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(options => {
        options.TokenLifespan = TimeSpan.FromMinutes(3);
      });
      builder.Services.AddMvc();
      builder.Services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();
      builder.Services.AddDbContextPool<AppDbContext>(options => {
        options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeApp"));
      });
      builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>(
          "CustomEmailConfirmation"
        );
      builder.Services.AddAuthorization(options => {
        options.AddPolicy("DeleteClaimPolicy", policy => {
          policy.RequireClaim("Delete Role")
                .RequireClaim("Edit Role");
        });
        options.AddPolicy("EditClaimPolicy", policy => {
          policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement());
        });
        options.AddPolicy("AdminRolePolicy", policy => {
          policy.RequireRole("Admin");
        });
        options.AddPolicy("ControllerRolePolicy", policy => {
          policy.RequireRole("Admin", "User");
        });
      });
      builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
      builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
      builder.Services.AddAuthentication()
        .AddGoogle(options => {
          options.ClientId = "521539481171-4lpu5hcppe2laf360m3q81i57k2gbc5j.apps.googleusercontent.com";
          options.ClientSecret = "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";
          options.CallbackPath = "/easylogin";
        })
        .AddFacebook(options => {
          options.AppId = "1181799203589679";
          options.AppSecret = "b020be69f06354ab52ab5d296774fb74";
        })
        .AddTwitter(options => {
          options.ConsumerKey = "CJuuWqODcLsAMrzGmvAUdBdWU";
          options.ConsumerSecret = "LInXolyl0EVY5EAyKZEmBDByKdOs6RqlUcD0r2DfEeV1GRSCJM";
          options.RetrieveUserDetails = true;
        });
      builder.Services.AddSingleton<DataProtectionPurposeStrings>();
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
