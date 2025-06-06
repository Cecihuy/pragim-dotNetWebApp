﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pragim_dotNetWebApp.Models {
  public class Employee {
    public int Id { get; set; }
    [NotMapped]
    public string EncryptedId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage ="Invalid Email Address")]
    [Display(Name ="Office Email")]
    public string Email { get; set; }
    [Required]
    public Dept? Department { get; set; }
    public string? PhotoPath { get; set; }
  }
}
