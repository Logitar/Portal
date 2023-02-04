using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using System.Globalization;

namespace Logitar.Portal.Domain.Realms
{
  public class Realm : AggregateRoot
  {
    public Realm(AggregateId actorId, string alias, UsernameSettings usernameSettings, PasswordSettings passwordSettings, string jwtSecret,
      string? displayName = null, string? description = null, CultureInfo? defaultLocale = null, string? url = null,
      bool requireConfirmedAccount = false, bool requireUniqueEmail = false, string? googleClientId = null) : base()
    {
      ApplyChange(new RealmCreatedEvent
      {
        Alias = alias.Trim(),
        DisplayName = displayName?.CleanTrim(),
        Description = description?.CleanTrim(),
        DefaultLocale = defaultLocale,
        Url = url?.CleanTrim(),
        RequireConfirmedAccount = requireConfirmedAccount,
        RequireUniqueEmail = requireUniqueEmail,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings,
        JwtSecret = jwtSecret.Trim(),
        GoogleClientId = googleClientId?.CleanTrim()
      }, actorId);
    }
    private Realm() : base()
    {
    }

    public string Alias { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }

    public CultureInfo? DefaultLocale { get; private set; }
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public UsernameSettings UsernameSettings { get; private set; } = new();
    public PasswordSettings PasswordSettings { get; private set; } = new();

    public AggregateId? PasswordRecoverySenderId { get; private set; }
    public AggregateId? PasswordRecoveryTemplateId { get; private set; }

    public string JwtSecret { get; private set; } = string.Empty;

    public string? GoogleClientId { get; private set; }

    public void Delete(AggregateId actorId) => ApplyChange(new RealmDeletedEvent(), actorId);
    public void Update(AggregateId actorId, UsernameSettings usernameSettings, PasswordSettings passwordSettings, string jwtSecret,
      string? displayName = null, string? description = null, CultureInfo? defaultLocale = null, string? url = null,
      bool requireConfirmedAccount = false, bool requireUniqueEmail = false,
      Sender? passwordRecoverySender = null, Template? passwordRecoveryTemplate = null,
      string? googleClientId = null)
    {
      ApplyChange(new RealmUpdatedEvent
      {
        DisplayName = displayName?.CleanTrim(),
        Description = description?.CleanTrim(),
        DefaultLocale = defaultLocale,
        Url = url?.CleanTrim(),
        RequireConfirmedAccount = requireConfirmedAccount,
        RequireUniqueEmail = requireUniqueEmail,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings,
        PasswordRecoverySenderId = passwordRecoverySender?.Id,
        PasswordRecoveryTemplateId = passwordRecoveryTemplate?.Id,
        JwtSecret = jwtSecret.Trim(),
        GoogleClientId = googleClientId?.CleanTrim()
      }, actorId);
    }

    protected virtual void Apply(RealmCreatedEvent @event)
    {
      Alias = @event.Alias;
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocale;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;

      JwtSecret = @event.JwtSecret;

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

      DefaultLocale = @event.DefaultLocale;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;

      PasswordRecoverySenderId = @event.PasswordRecoverySenderId;
      PasswordRecoveryTemplateId = @event.PasswordRecoveryTemplateId;

      JwtSecret = @event.JwtSecret;

      GoogleClientId = @event.GoogleClientId;
    }

    public override string ToString() => $"{DisplayName} | {base.ToString()}";
  }
}
