using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.ViewModels {
  public class AddPasswordViewModel {
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    [Compare("NewPassword", ErrorMessage =
      "The new password and confirmation password does not match")]
    public string ConfirmPassword { get; set; }
  }
}
