using Logitar.Portal.Core.Emails.Senders;
using Logitar.Portal.Core.Emails.Templates;
using Logitar.Portal.Core.Realms.Events;
using Logitar.Portal.Core.Realms.Payloads;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Payloads;
using System.Globalization;
using System.Text.Json;

namespace Logitar.Portal.Core.Realms
{
  public class Realm : Aggregate
  {
    public Realm(CreateRealmPayload payload, Guid userId)
    {
      ApplyChange(new CreatedEvent(payload, userId));
    }
    private Realm()
    {
    }

    public string Alias { get; private set; } = null!;
    public string AliasNormalized
    {
      get => Alias.ToUpper();
      private set { /* EntityFrameworkCore only setter */ }
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public string? AllowedUsernameCharacters { get; private set; }
    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public CultureInfo? DefaultCulture => DefaultLocale == null ? null : CultureInfo.GetCultureInfo(DefaultLocale);
    public string? DefaultLocale { get; private set; }
    public string? Url { get; private set; }

    public Sender? PasswordRecoverySender
    {
      get => PasswordRecoverySenderRelation?.Sender;
      set => PasswordRecoverySenderRelation = value == null ? null : new PasswordRecoverySender(this, value);
    }
    /// <summary>
    /// EntityFrameworkCore only property
    /// </summary>
    public PasswordRecoverySender? PasswordRecoverySenderRelation { get; private set; }
    public Template? PasswordRecoveryTemplate
    {
      get => PasswordRecoveryTemplateRelation?.Template;
      set => PasswordRecoveryTemplateRelation = value == null ? null : new PasswordRecoveryTemplate(this, value);
    }
    /// <summary>
    /// EntityFrameworkCore only property
    /// </summary>
    public PasswordRecoveryTemplate? PasswordRecoveryTemplateRelation { get; private set; }

    public PasswordSettings? PasswordSettings { get; private set; }
    public string? PasswordSettingsSerialized
    {
      get => PasswordSettings == null ? null : JsonSerializer.Serialize(PasswordSettings);
      private set => PasswordSettings = value == null ? null : JsonSerializer.Deserialize<PasswordSettings>(value);
    }

    public string? GoogleClientId { get; private set; }

    public List<Sender> Senders { get; private set; } = new();
    public List<Template> Templates { get; private set; } = new();
    public List<User> Users { get; private set; } = new();

    public void Delete(Guid userId) => ApplyChange(new DeletedEvent(userId));
    public void Update(UpdateRealmPayload payload, Guid userId) => ApplyChange(new UpdatedEvent(payload, userId));

    protected virtual void Apply(CreatedEvent @event)
    {
      Alias = @event.Payload.Alias;

      Apply(@event.Payload);
    }
    protected virtual void Apply(DeletedEvent @event)
    {
    }
    protected virtual void Apply(UpdatedEvent @event)
    {
      Apply(@event.Payload);
    }

    private void Apply(SaveRealmPayload payload)
    {
      Name = payload.Name.Trim();
      Description = payload.Description?.CleanTrim();

      AllowedUsernameCharacters = payload.AllowedUsernameCharacters == null
        ? null
        : new string(payload.AllowedUsernameCharacters.ToCharArray().Distinct().ToArray());
      RequireConfirmedAccount = payload.RequireConfirmedAccount;
      RequireUniqueEmail = payload.RequireUniqueEmail;

      DefaultLocale = payload.DefaultLocale;
      Url = payload.Url;

      PasswordSettingsPayload? password = payload.PasswordSettings;
      PasswordSettings = password == null
        ? null
        : new PasswordSettings(password.RequiredLength, password.RequiredUniqueChars,
            password.RequireNonAlphanumeric, password.RequireLowercase, password.RequireUppercase, password.RequireDigit);

      GoogleClientId = payload.GoogleClientId?.CleanTrim();
    }

    public override string ToString() => $"{Name} | {base.ToString()}";
  }
}
