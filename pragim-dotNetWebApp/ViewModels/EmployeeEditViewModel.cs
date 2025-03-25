using pragim_dotNetWebApp.Models;

namespace pragim_dotNetWebApp.ViewModels {
  public class EmployeeEditViewModel : EmployeeCreateViewModel{
    public int Id { get; set; }
    public string? ExistingPhotoPath { get; set; }
  }
}
