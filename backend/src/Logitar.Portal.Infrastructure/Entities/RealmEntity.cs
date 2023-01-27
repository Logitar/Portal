using Logitar.Portal.Core.Realms.Events;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class RealmEntity : AggregateEntity
  {
    public RealmEntity(RealmCreatedEvent @event)
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

      GoogleClientId = @event.GoogleClientId;
    }
    private RealmEntity()
    {
    }

    public int RealmId { get; private set; }

    public string Alias { get; private set; } = null!;
    public string AliasNormalized
    {
      get => Alias.ToUpper();
      private set { }
    }
    public string DisplayName { get; private set; } = null!;
    public string? Description { get; private set; }

    public string? DefaultLocale { get; private set; }
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public string? UsernameSettings { get; private set; }
    public string? PasswordSettings { get; private set; }

    public int? PasswordRecoverySenderId { get; private set; }
    public int? PasswordRecoveryTemplateId { get; private set; }

    public string? GoogleClientId { get; private set; }

    public void Update(RealmUpdatedEvent @event)
    {
      DisplayName = @event.DisplayName;
      Description = @event.Description;

      DefaultLocale = @event.DefaultLocale;
      Url = @event.Url;

      RequireConfirmedAccount = @event.RequireConfirmedAccount;
      RequireUniqueEmail = @event.RequireUniqueEmail;

      UsernameSettings = JsonSerializer.Serialize(@event.UsernameSettings);
      PasswordSettings = JsonSerializer.Serialize(@event.PasswordSettings);

      // TODO(fpion): PasswordRecoverySenderId
      // TODO(fpion): PasswordRecoveryTemplateId

      GoogleClientId = @event.GoogleClientId;
    }
  }
}
