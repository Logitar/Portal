using Logitar.Portal.Domain.Realms.Events;
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

      DefaultLocale = @event.DefaultLocaleName;
      JwtSecret = @event.JwtSecret;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = JsonSerializer.Serialize(@event.UsernameSettings);
      PasswordSettings = JsonSerializer.Serialize(@event.PasswordSettings);

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

    public string? DefaultLocale { get; private set; }
    public string JwtSecret { get; private set; } = string.Empty;
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public string UsernameSettings { get; private set; } = string.Empty;
    public string PasswordSettings { get; private set; } = string.Empty;

    public string? GoogleClientId { get; private set; }

    //public string? PasswordRecoverySenderId { get; private set; } // TODO(fpion): implement when Senders are completed
    //public string? PasswordRecoveryTemplateId { get; private set; } // TODO(fpion): implement when Templates are completed

    public List<ExternalProviderEntity> ExternalProviders { get; private set; } = new();
    public List<UserEntity> Users { get; private set; } = new();

    public void Update(RealmUpdatedEvent @event)
    {
      base.Update(@event);

      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocaleName;
      JwtSecret = @event.JwtSecret;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = JsonSerializer.Serialize(@event.UsernameSettings);
      PasswordSettings = JsonSerializer.Serialize(@event.PasswordSettings);

      GoogleClientId = @event.GoogleClientId;
    }
  }
}
