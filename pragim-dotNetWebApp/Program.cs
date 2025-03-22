using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace pragim_dotNetWebApp {
  public class Program {
    public static void Main(string[] args) {
      /* =================================== variable =================================== */
      FileServerOptions fileServerOptions = new FileServerOptions();
      fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
      fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("foo.html");
      /* =================================== services =================================== */
      var builder = WebApplication.CreateBuilder(args);
      /* =================================== pipeline =================================== */
      var app = builder.Build();
      app.UseFileServer(fileServerOptions);
      app.Use(async (context, next) => {
        await context.Response.WriteAsync("Hello World");
        await next();
      });
      app.Run();
    }
  }
}
