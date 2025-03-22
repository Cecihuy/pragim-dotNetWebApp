using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace pragim_dotNetWebApp {
  public class Program {
    public static void Main(string[] args) {
      /* =================================== variable =================================== */
      ExceptionHandlerOptions exceptionHandlerOptions = new ExceptionHandlerOptions { 
        ExceptionHandlingPath = "/foo.html" // if an error occured, go to this path
      };
      DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions {
        SourceCodeLineCount = 10
      };
      /* =================================== services =================================== */
        var builder = WebApplication.CreateBuilder(args);
      /* =================================== pipeline =================================== */
      var app = builder.Build();
      app.UseExceptionHandler(exceptionHandlerOptions);
      app.UseDeveloperExceptionPage(developerExceptionPageOptions);
      app.UseFileServer();
      app.Use(async (context, next) => {
        throw new Exception("Some error processing the request");
        await context.Response.WriteAsync("Hello World");
        await next();
      });      
      app.Run();
    }
  }
}
