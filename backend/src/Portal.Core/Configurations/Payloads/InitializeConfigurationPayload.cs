using Portal.Core.Users.Payloads;
using System.ComponentModel.DataAnnotations;

namespace Portal.Core.Configurations.Payloads
{
  public class InitializeConfigurationPayload
  {
    [Required]
    public CreateUserPayload User { get; set; } = null!;
  }
}
