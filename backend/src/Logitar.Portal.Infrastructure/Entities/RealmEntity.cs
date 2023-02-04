using Logitar.Portal.Domain.Realms.Events;
using System.Globalization;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class RealmEntity : AggregateEntity
  {
    public RealmEntity(RealmCreatedEvent @event) : base(@event)
    {
      Alias = @event.Alias;
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocale;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = JsonSerializer.Serialize(@event.UsernameSettings);
      PasswordSettings = JsonSerializer.Serialize(@event.PasswordSettings);

      JwtSecret = @event.JwtSecret;

      GoogleClientId = @event.GoogleClientId;
    }
    private RealmEntity() : base()
    {
    }

    public int RealmId { get; private set; }

    public string Alias { get; private set; } = string.Empty;
    public string AliasNormalized
    {
      get => Alias.ToUpper();
      private set { }
    }
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }

    public CultureInfo? DefaultLocale { get; private set; }
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public string UsernameSettings { get; private set; } = string.Empty;
    public string PasswordSettings { get; private set; } = string.Empty;

    public SenderEntity? PasswordRecoverySender { get; private set; }
    public int? PasswordRecoverySenderId { get; private set; }

    public TemplateEntity? PasswordRecoveryTemplate { get; private set; }
    public int? PasswordRecoveryTemplateId { get; private set; }

    public string JwtSecret { get; private set; } = string.Empty;

    public string? GoogleClientId { get; private set; }

    public List<DictionaryEntity> Dictionaries { get; private set; } = new();
    public List<ExternalProviderEntity> ExternalProviders { get; private set; } = new();
    public List<SenderEntity> Senders { get; private set; } = new();
    public List<TemplateEntity> Templates { get; private set; } = new();
    public List<UserEntity> Users { get; private set; } = new();

    public void Update(RealmUpdatedEvent @event, SenderEntity? passwordRecoverySender, TemplateEntity? passwordRecoveryTemplate)
    {
      base.Update(@event);

      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocale;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = JsonSerializer.Serialize(@event.UsernameSettings);
      PasswordSettings = JsonSerializer.Serialize(@event.PasswordSettings);

      PasswordRecoverySender = passwordRecoverySender;
      PasswordRecoverySenderId = passwordRecoverySender?.SenderId;

      PasswordRecoveryTemplate = passwordRecoveryTemplate;
      PasswordRecoveryTemplateId = passwordRecoveryTemplate?.TemplateId;

      JwtSecret = @event.JwtSecret;

      GoogleClientId = @event.GoogleClientId;
    }
  }
}
