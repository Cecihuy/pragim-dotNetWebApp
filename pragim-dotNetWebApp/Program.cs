using Microsoft.AspNetCore.Builder;

namespace pragim_dotNetWebApp {
  public class Program {
    public static void Main(string[] args) {
      /* =================================== services =================================== */
      var builder = WebApplication.CreateBuilder(args);
      string? myKey = builder.Configuration["MyKey"];
      /* =================================== pipeline =================================== */
      var app = builder.Build();
      app.UseDeveloperExceptionPage();
      app.MapGet("/", () => myKey);
      app.Run();
    }
  }
}
