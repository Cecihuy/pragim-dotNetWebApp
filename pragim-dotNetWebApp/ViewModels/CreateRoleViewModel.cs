using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.ViewModels {
  public class CreateRoleViewModel {
    [Required]
    public string RoleName { get; set; }
  }
}
