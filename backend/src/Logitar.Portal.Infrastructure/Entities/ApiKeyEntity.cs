using Logitar.Portal.Domain.ApiKeys.Events;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class ApiKeyEntity : AggregateEntity
  {
    public ApiKeyEntity(ApiKeyCreatedEvent @event) : base(@event)
    {
      SecretHash = @event.SecretHash;

      DisplayName = @event.DisplayName;
      Description = @event.Description;

      ExpiresOn = @event.ExpiresOn;
    }
    private ApiKeyEntity() : base()
    {
    }

    public int ApiKeyId { get; private set; }

    public string SecretHash { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public DateTime? ExpiresOn { get; private set; }

    public void Update(ApiKeyUpdatedEvent @event)
    {
      base.Update(@event);

      DisplayName = @event.DisplayName;
      Description = @event.Description;
    }
  }
}
