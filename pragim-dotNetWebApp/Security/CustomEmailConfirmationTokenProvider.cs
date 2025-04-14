using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace pragim_dotNetWebApp.Security {
  public class CustomEmailConfirmationTokenProvider<TUser> 
    : DataProtectorTokenProvider<TUser> where TUser : class {
    public CustomEmailConfirmationTokenProvider(
      IDataProtectionProvider dataProtectionProvider, 
      IOptions<CustomEmailConfirmationTokenProviderOptions> options,
      ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger
    ) : base(dataProtectionProvider, options, logger) {
      
    }
  }
}
