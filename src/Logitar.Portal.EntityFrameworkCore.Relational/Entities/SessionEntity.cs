using Logitar.Portal.Domain.Sessions.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record SessionEntity : AggregateEntity
{
  public SessionEntity(SessionCreatedEvent created, UserEntity user) : base(created)
  {
    UserId = user.UserId;
    User = user;

    Secret = created.Secret?.Encode();

    IsActive = true;
  }

  private SessionEntity() : base()
  {
  }

  public int SessionId { get; private set; }

  public int UserId { get; private set; }
  public UserEntity? User { get; private set; }

  public bool IsPersistent
  {
    get => Secret != null;
    private set { }
  }
  public string? Secret { get; private set; }

  public bool IsActive { get; private set; }
  public string? SignedOutBy { get; private set; }
  public DateTime? SignedOutOn { get; private set; }

  public Dictionary<string, string> CustomAttributes { get; private set; } = new();
  public string? CustomAttributesSerialized
  {
    get => CustomAttributes.Any() ? JsonSerializer.Serialize(CustomAttributes) : null;
    private set
    {
      if (value == null)
      {
        CustomAttributes.Clear();
      }
      else
      {
        CustomAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new();
      }
    }
  }

  public void Update(SessionUpdatedEvent updated)
  {
    base.Update(updated);

    foreach (KeyValuePair<string, string?> customAttribute in updated.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        CustomAttributes.Remove(customAttribute.Key);
      }
      else
      {
        CustomAttributes[customAttribute.Key] = customAttribute.Value;
      }
    }
  }
}
