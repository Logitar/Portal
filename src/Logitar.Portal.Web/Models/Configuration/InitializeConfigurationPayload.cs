using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Web.Models.Configuration;

public record InitializeConfigurationPayload
{
  public string Locale { get; set; } = string.Empty;
  public UserPayload User { get; set; } = new();
}
