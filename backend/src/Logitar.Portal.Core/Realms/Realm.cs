using Logitar.Portal.Core.Realms.Events;
using Logitar.Portal.Core.Users;
using System.Globalization;

namespace Logitar.Portal.Core.Realms
{
  public class Realm : AggregateRoot
  {
    public Realm(string alias, string displayName, string? description, CultureInfo? defaultLocale, string? url,
      bool requireConfirmedAccount, bool requireUniqueEmail, UsernameSettings usernameSettings, PasswordSettings passwordSettings,
      string? googleClientId, AggregateId userId)
    {
      ApplyChange(new RealmCreatedEvent
      {
        Alias = alias.Trim(),
        DisplayName = displayName.Trim(),
        Description = description?.CleanTrim(),
        DefaultLocale = defaultLocale?.Name,
        Url = url?.CleanTrim(),
        RequireConfirmedAccount = requireConfirmedAccount,
        RequireUniqueEmail = requireUniqueEmail,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings,
        GoogleClientId = googleClientId?.CleanTrim()
      }, userId);
    }
    private Realm()
    {
    }

    public string Alias { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string? Description { get; private set; }

    public CultureInfo? DefaultLocale { get; private set; }
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public UsernameSettings UsernameSettings { get; private set; } = null!;
    public PasswordSettings PasswordSettings { get; private set; } = null!;

    public AggregateId? PasswordRecoverySenderId { get; private set; }
    public AggregateId? PasswordRecoveryTemplateId { get; private set; }

    public string? GoogleClientId { get; private set; }

    public void Delete(AggregateId userId) => ApplyChange(new RealmDeletedEvent(), userId);
    public void Update(string displayName, string? description, CultureInfo? defaultLocale, string? url,
      bool requireConfirmedAccount, bool requireUniqueEmail, UsernameSettings usernameSettings, PasswordSettings passwordSettings,
      AggregateId? passwordRecoverySenderId, AggregateId? passwordRecoveryTemplateId,
      string? googleClientId, AggregateId userId)
    {
      ApplyChange(new RealmUpdatedEvent
      {
        DisplayName = displayName.Trim(),
        Description = description?.CleanTrim(),
        DefaultLocale = defaultLocale?.Name,
        Url = url?.CleanTrim(),
        RequireConfirmedAccount = requireConfirmedAccount,
        RequireUniqueEmail = requireUniqueEmail,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings,
        PasswordRecoverySenderId = passwordRecoverySenderId?.ToString(),
        PasswordRecoveryTemplateId = passwordRecoveryTemplateId?.ToString(),
        GoogleClientId = googleClientId?.CleanTrim()
      }, userId);
    }

    protected virtual void Apply(RealmCreatedEvent @event)
    {
      Alias = @event.Alias;
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocale == null ? null : CultureInfo.GetCultureInfo(@event.DefaultLocale);
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;

      GoogleClientId = @event.GoogleClientId;
    }
    protected virtual void Apply(RealmDeletedEvent @event)
    {
      IsDeleted = true;
    }
    protected virtual void Apply(RealmUpdatedEvent @event)
    {
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocale == null ? null : CultureInfo.GetCultureInfo(@event.DefaultLocale);
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;

      PasswordRecoverySenderId = @event.PasswordRecoverySenderId == null ? null : new AggregateId(@event.PasswordRecoverySenderId);
      PasswordRecoveryTemplateId = @event.PasswordRecoveryTemplateId == null ? null : new AggregateId(@event.PasswordRecoveryTemplateId);

      GoogleClientId = @event.GoogleClientId;
    }

    public override string ToString() => $"{DisplayName} | {base.ToString()}";
  }
}
