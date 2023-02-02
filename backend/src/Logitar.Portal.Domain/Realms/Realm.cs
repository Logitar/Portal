using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Domain.Users;
using System.Globalization;

namespace Logitar.Portal.Domain.Realms
{
  public class Realm : AggregateRoot
  {
    public Realm(AggregateId userId, string alias, string jwtSecret, UsernameSettings usernameSettings, PasswordSettings passwordSettings,
      string? displayName = null, string? description = null, CultureInfo? defaultLocale = null, string? url = null,
      bool requireConfirmedAccount = false, bool requireUniqueEmail = false, string? googleClientId = null) : base()
    {
      ApplyChange(new RealmCreatedEvent
      {
        Alias = alias.Trim(),
        DisplayName = displayName?.CleanTrim(),
        Description = description?.CleanTrim(),
        DefaultLocaleName = defaultLocale?.Name,
        JwtSecret = jwtSecret.Trim(),
        Url = url?.CleanTrim(),
        RequireConfirmedAccount = requireConfirmedAccount,
        RequireUniqueEmail = requireUniqueEmail,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings,
        GoogleClientId = googleClientId?.CleanTrim()
      }, userId);
    }
    private Realm() : base()
    {
    }

    public string Alias { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }

    public CultureInfo? DefaultLocale { get; private set; }
    public string JwtSecret { get; private set; } = string.Empty;
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public UsernameSettings UsernameSettings { get; private set; } = new();
    public PasswordSettings PasswordSettings { get; private set; } = new();

    public string? GoogleClientId { get; private set; }

    public void Delete(AggregateId userId) => ApplyChange(new RealmDeletedEvent(), userId);
    public void Update(AggregateId userId, string jwtSecret, UsernameSettings usernameSettings, PasswordSettings passwordSettings,
      string? displayName = null, string? description = null, CultureInfo? defaultLocale = null, string? url = null,
      bool requireConfirmedAccount = false, bool requireUniqueEmail = false, string? googleClientId = null)
    {
      ApplyChange(new RealmUpdatedEvent
      {
        DisplayName = displayName?.CleanTrim(),
        Description = description?.CleanTrim(),
        DefaultLocaleName = defaultLocale?.Name,
        JwtSecret = jwtSecret.Trim(),
        Url = url?.CleanTrim(),
        RequireConfirmedAccount = requireConfirmedAccount,
        RequireUniqueEmail = requireUniqueEmail,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings,
        GoogleClientId = googleClientId?.CleanTrim()
      }, userId);
    }

    protected virtual void Apply(RealmCreatedEvent @event)
    {
      Alias = @event.Alias;
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocaleName?.GetCultureInfo();
      JwtSecret = @event.JwtSecret;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;

      GoogleClientId = @event.GoogleClientId;
    }
    protected virtual void Apply(RealmDeletedEvent @event)
    {
      Delete();
    }
    protected virtual void Apply(RealmUpdatedEvent @event)
    {
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocaleName?.GetCultureInfo();
      JwtSecret = @event.JwtSecret;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;

      GoogleClientId = @event.GoogleClientId;
    }

    public override string ToString() => $"{DisplayName} | {base.ToString()}";
  }
}
