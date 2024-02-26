namespace Logitar.Portal.Contracts.Configurations;

public record InitializeConfigurationPayload
{
  public string? DefaultLocale { get; set; }
  public UserPayload User { get; set; }
  public SessionPayload Session { get; set; }

  public InitializeConfigurationPayload() : this(new UserPayload(), new SessionPayload())
  {
  }

  public InitializeConfigurationPayload(UserPayload user, SessionPayload session)
  {
    User = user;
    Session = session;
  }
}
