using Logitar.Portal.Domain.ApiKeys.Events;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class ApiKeyEntity : AggregateEntity
  {
    public ApiKeyEntity(ApiKeyCreatedEvent @event, Actor actor) : base(@event, actor)
    {
      SecretHash = @event.SecretHash;

      Title = @event.Title;
      Description = @event.Description;

      ExpiresOn = @event.ExpiresOn;
    }
    private ApiKeyEntity() : base()
    {
    }

    public int ApiKeyId { get; private set; }

    public string SecretHash { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public DateTime? ExpiresOn { get; private set; }

    public void Update(ApiKeyUpdatedEvent @event, Actor actor)
    {
      base.Update(@event, actor);

      Title = @event.Title;
      Description = @event.Description;
    }
  }
}
