using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Configuration;

public record InitializeConfigurationResult
{
  public InitializeConfigurationResult(Contracts.Configurations.InitializeConfigurationResult result)
  {
    Configuration = result.Configuration;
    User = result.Session.User;
  }

  public Contracts.Configurations.Configuration Configuration { get; set; }
  public User User { get; set; }
}
