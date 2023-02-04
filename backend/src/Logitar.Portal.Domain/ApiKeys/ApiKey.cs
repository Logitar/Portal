﻿using Logitar.Portal.Domain.ApiKeys.Events;

namespace Logitar.Portal.Domain.ApiKeys
{
  public class ApiKey : AggregateRoot
  {
    public ApiKey(AggregateId userId, string secretHash, string title, string? description = null, DateTime? expiresOn = null) : base()
    {
      ApplyChange(new ApiKeyCreatedEvent
      {
        SecretHash = secretHash,
        Title = title.Trim(),
        Description = description?.CleanTrim(),
        ExpiresOn = expiresOn
      }, userId);
    }
    private ApiKey() : base()
    {
    }

    public string SecretHash { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public DateTime? ExpiresOn { get; private set; }

    public bool IsExpired(DateTime? expiredOn = null) => ExpiresOn <= (expiredOn ?? DateTime.UtcNow);

    public void Delete(AggregateId userId) => ApplyChange(new ApiKeyDeletedEvent(), userId);
    public void Update(AggregateId userId, string title, string? description = null) => ApplyChange(new ApiKeyUpdatedEvent
    {
      Title = title.Trim(),
      Description = description?.CleanTrim()
    }, userId);

    protected virtual void Apply(ApiKeyCreatedEvent @event)
    {
      SecretHash = @event.SecretHash;

      Title = @event.Title;
      Description = @event.Description;

      ExpiresOn = @event.ExpiresOn;
    }
    protected virtual void Apply(ApiKeyDeletedEvent @event)
    {
      Delete();
    }
    protected virtual void Apply(ApiKeyUpdatedEvent @event)
    {
      Title = @event.Title;
      Description = @event.Description;
    }

    public override string ToString() => $"{Title} | {base.ToString()}";
  }
}
