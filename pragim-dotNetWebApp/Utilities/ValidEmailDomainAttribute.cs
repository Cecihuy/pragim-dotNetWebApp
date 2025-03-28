using System.ComponentModel.DataAnnotations;

namespace pragim_dotNetWebApp.Utilities {
  public class ValidEmailDomainAttribute : ValidationAttribute {
    private readonly string allowedDomain;

    public ValidEmailDomainAttribute(string allowedDomain) {
      this.allowedDomain=allowedDomain;
    }
    // object? value parameter automatically added from Email input
    public override bool IsValid(object? value) {
      string[] strings = value.ToString().Split('@');
      return strings[1].ToUpper() == allowedDomain.ToUpper();
    }
  }
}
