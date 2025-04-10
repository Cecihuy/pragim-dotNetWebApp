using Microsoft.AspNetCore.Identity;

namespace pragim_dotNetWebApp.Models {
  public class ApplicationUser : IdentityUser{
    public string? City { get; set; }
  }
}
