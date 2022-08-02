using Logitar.Portal.Core.Users.Payloads;
using System.ComponentModel.DataAnnotations;

namespace Logitar.Portal.Core.Configurations.Payloads
{
  public class InitializeConfigurationPayload
  {
    [Required]
    public CreateUserPayload User { get; set; } = null!;
  }
}
