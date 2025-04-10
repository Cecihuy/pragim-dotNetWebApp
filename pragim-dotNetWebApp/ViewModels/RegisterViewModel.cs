using Microsoft.AspNetCore.Mvc;
using pragim_dotNetWebApp.Utilities;
using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.ViewModels {
  public class RegisterViewModel {
    [Required]
    [EmailAddress]
    [ValidEmailDomain(allowedDomain: "pragimtech.com", 
      ErrorMessage = "Email domain must be pragimtech.com")]
    [Remote("isemailinuse","account")]
    public string Email { get; set; }
    [Required][DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Password does not match")]
    public string ConfirmPassword { get; set; }
    public string? City { get; set; }
  }
}
