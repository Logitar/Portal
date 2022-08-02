namespace Logitar.Portal.Core
{
  public abstract class EventBase
  {
    protected EventBase(Guid userId)
    {
      OccurredAt = DateTime.UtcNow;
      UserId = userId;
    }

    public DateTime OccurredAt { get; set; } // Public setter for deserialization
    public Guid UserId { get; private set; }
  }

  public abstract class CreatedEventBase : EventBase
  {
    protected CreatedEventBase(Guid userId) : base(userId)
    {
    }
  }

  public abstract class DeletedEventBase : EventBase
  {
    protected DeletedEventBase(Guid userId) : base(userId)
    {
    }
  }

  public abstract class UpdatedEventBase : EventBase
  {
    protected UpdatedEventBase(Guid userId) : base(userId)
    {
    }
  }
}
