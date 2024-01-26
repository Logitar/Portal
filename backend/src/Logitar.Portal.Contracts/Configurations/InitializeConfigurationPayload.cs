namespace Logitar.Portal.Contracts.Configurations;

public record InitializeConfigurationPayload
{
  public string Locale { get; set; }
  public UserPayload User { get; set; }
  public SessionPayload Session { get; set; }

  public InitializeConfigurationPayload() : this(string.Empty, new UserPayload(), new SessionPayload())
  {
  }

  public InitializeConfigurationPayload(string locale, UserPayload user, SessionPayload session)
  {
    Locale = locale;
    User = user;
    Session = session;
  }
}
