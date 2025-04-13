using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.ViewModels {
  public class ForgotPasswordViewModel {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
