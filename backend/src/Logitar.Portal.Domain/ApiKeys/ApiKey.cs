using Logitar.Portal.Domain.ApiKeys.Events;

namespace Logitar.Portal.Domain.ApiKeys
{
  public class ApiKey : AggregateRoot
  {
    public ApiKey(AggregateId userId, string secretHash, string displayName, string? description = null, DateTime? expiresOn = null) : base()
    {
      ApplyChange(new ApiKeyCreatedEvent
      {
        SecretHash = secretHash,
        DisplayName = displayName.Trim(),
        Description = description?.CleanTrim(),
        ExpiresOn = expiresOn
      }, userId);
    }
    private ApiKey() : base()
    {
    }

    public string SecretHash { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public DateTime? ExpiresOn { get; private set; }

    public bool IsExpired(DateTime? expiredOn = null) => ExpiresOn <= (expiredOn ?? DateTime.UtcNow);

    public void Delete(AggregateId userId) => ApplyChange(new ApiKeyDeletedEvent(), userId);
    public void Update(AggregateId userId, string displayName, string? description = null) => ApplyChange(new ApiKeyUpdatedEvent
    {
      DisplayName = displayName.Trim(),
      Description = description?.CleanTrim()
    }, userId);

    protected virtual void Apply(ApiKeyCreatedEvent @event)
    {
      SecretHash = @event.SecretHash;

      DisplayName = @event.DisplayName;
      Description = @event.Description;

      ExpiresOn = @event.ExpiresOn;
    }
    protected virtual void Apply(ApiKeyDeletedEvent @event)
    {
      Delete();
    }
    protected virtual void Apply(ApiKeyUpdatedEvent @event)
    {
      DisplayName = @event.DisplayName;
      Description = @event.Description;
    }

    public override string ToString() => $"{DisplayName} | {base.ToString()}";
  }
}
