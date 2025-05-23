﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.ViewModels {
  public class EditRoleViewModel {
    public EditRoleViewModel() {
      Users = new List<string>();
    }
    public string Id { get; set; }
    [Required(ErrorMessage = "Role name is required")]
    public string RoleName { get; set; }
    public List<string> Users { get; set; }
  }
}
