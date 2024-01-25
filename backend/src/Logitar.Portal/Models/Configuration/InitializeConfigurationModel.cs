using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Models.Configuration;

public record InitializeConfigurationModel
{
  public string Locale { get; set; }
  public UserPayload User { get; set; }

  public InitializeConfigurationModel() : this(string.Empty, new UserPayload())
  {
  }

  public InitializeConfigurationModel(string locale, UserPayload user)
  {
    Locale = locale;
    User = user;
  }

  public InitializeConfigurationPayload ToPayload(IEnumerable<CustomAttribute> customAttributes)
  {
    SessionPayload session = new();
    session.CustomAttributes.AddRange(customAttributes);

    return new InitializeConfigurationPayload(Locale, User, session);
  }
}
