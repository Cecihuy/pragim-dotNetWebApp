﻿using System.Collections.Generic;

namespace pragim_dotNetWebApp.ViewModels {
  public class UserClaimsViewModel {
    public UserClaimsViewModel() {
      Claims = new List<UserClaim>();
    }
    public string UserId { get; set; }
    public List<UserClaim> Claims { get; set; }
  }
}
