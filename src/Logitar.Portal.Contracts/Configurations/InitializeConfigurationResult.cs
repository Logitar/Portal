using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Configurations;

public record InitializeConfigurationResult
{
  public InitializeConfigurationResult() : this(new(), new())
  {
  }
  public InitializeConfigurationResult(Configuration configuration, User user)
  {
    Configuration = configuration;
    User = user;
  }

  public Configuration Configuration { get; set; }
  public User User { get; set; }
}
