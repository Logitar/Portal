using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Web.Models.Configuration;

public record InitializeConfigurationModel
{
  public string? DefaultLocale { get; set; }
  public UserPayload User { get; set; }

  public InitializeConfigurationModel() : this(new UserPayload())
  {
  }

  public InitializeConfigurationModel(UserPayload user)
  {
    User = user;
  }

  public InitializeConfigurationPayload ToPayload(IEnumerable<CustomAttribute> sessionCustomAttributes)
  {
    return new InitializeConfigurationPayload(User, new SessionPayload(sessionCustomAttributes))
    {
      DefaultLocale = DefaultLocale
    };
  }
}
