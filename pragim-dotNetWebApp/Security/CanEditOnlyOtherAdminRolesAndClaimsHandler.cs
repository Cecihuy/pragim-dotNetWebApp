using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pragim_dotNetWebApp.Security {
  public class CanEditOnlyOtherAdminRolesAndClaimsHandler 
    : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement> {
    protected override Task HandleRequirementAsync(
      AuthorizationHandlerContext context,
      ManageAdminRolesAndClaimsRequirement requirement
    ) {
      var authorizationFilterContext = context.Resource as AuthorizationFilterContext;
      if(authorizationFilterContext == null) {
        return Task.CompletedTask;
      }
      string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
      StringValues adminIdBeingEdited = authorizationFilterContext.HttpContext.Request.Query["userId"];
      if(
        context.User.IsInRole("Admin") &&
        context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true") &&
        adminIdBeingEdited.ToString().ToLower() != loggedInAdminId.ToLower()
      ) {
        context.Succeed(requirement);
      }
      return Task.CompletedTask;
    }
  }
}
