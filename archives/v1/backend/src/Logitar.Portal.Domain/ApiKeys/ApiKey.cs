using Logitar.Portal.Core;
using Logitar.Portal.Core.ApiKeys.Payloads;
using Logitar.Portal.Domain.ApiKeys.Events;

namespace Logitar.Portal.Domain.ApiKeys
{
  public class ApiKey : Aggregate
  {
    public ApiKey(string keyHash, CreateApiKeyPayload payload, Guid userId)
    {
      ApplyChange(new CreatedEvent(keyHash, payload, userId));
    }
    private ApiKey()
    {
    }

    public string KeyHash { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public DateTime? ExpiresAt { get; private set; }
    public bool IsExpired
    {
      get => ExpiresAt <= DateTime.UtcNow;
      set { /* EntityFrameworkCore only setter */ }
    }

    public void Delete(Guid userId) => ApplyChange(new DeletedEvent(userId));
    public void Update(UpdateApiKeyPayload payload, Guid userId) => ApplyChange(new UpdatedEvent(payload, userId));

    protected virtual void Apply(CreatedEvent @event)
    {
      ExpiresAt = @event.Payload.ExpiresAt;
      KeyHash = @event.KeyHash;

      Apply(@event.Payload);
    }
    protected virtual void Apply(DeletedEvent @event)
    {
    }
    protected virtual void Apply(UpdatedEvent @event)
    {
      Apply(@event.Payload);
    }

    private void Apply(SaveApiKeyPayload payload)
    {
      Name = payload.Name.Trim();
      Description = payload.Description?.CleanTrim();
    }

    public override string ToString() => $"{Name} | {base.ToString()}";
  }
}
