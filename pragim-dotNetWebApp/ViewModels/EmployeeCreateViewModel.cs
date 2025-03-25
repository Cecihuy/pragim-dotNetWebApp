using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.Models {
  public class EmployeeCreateViewModel {
    [Required]
    public string Name { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid Email Address")]
    [Display(Name = "Office Email")]
    public string Email { get; set; }
    [Required]
    public Dept? Department { get; set; }
    public IFormFile? Photo { get; set; }
  }
}
