using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.ViewModels {
  public class ChangePasswordViewModel {
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }
    [DataType(DataType.Password)]
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "Password and Confirm New Password does not match")]
    public string ConfirmPassword { get; set; }
  }
}
