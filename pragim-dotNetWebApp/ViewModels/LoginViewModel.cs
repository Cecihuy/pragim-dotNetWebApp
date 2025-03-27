using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.ViewModels {
  public class LoginViewModel {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
  }
}
