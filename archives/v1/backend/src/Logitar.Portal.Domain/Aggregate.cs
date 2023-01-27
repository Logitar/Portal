using System.Reflection;

namespace Logitar.Portal.Domain
{
  public abstract class Aggregate
  {
    private readonly List<EventBase> _changes = new();

    protected Aggregate()
    {
      Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public int Sid { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public Guid CreatedById { get; private set; }

    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedById { get; private set; }
    public bool IsDeleted => DeletedAt.HasValue && DeletedById.HasValue;

    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedById { get; private set; }

    public int Version { get; private set; }

    public IReadOnlyCollection<EventBase> Changes => _changes.AsReadOnly();
    public bool HasChanges => _changes.Any();

    public void ClearChanges() => _changes.Clear();

    protected void ApplyChange(EventBase change)
    {
      ArgumentNullException.ThrowIfNull(change);

      Dispatch(change);

      _changes.Add(change);
    }
    private void Dispatch(EventBase @event)
    {
      Type aggregateType = GetType();
      Type eventType = @event.GetType();

      MethodInfo method = aggregateType.GetTypeInfo()
        .GetMethod("Apply", BindingFlags.Instance | BindingFlags.NonPublic, new[] { eventType })
        ?? throw new EventNotSupportedException(aggregateType, eventType);

      method.Invoke(this, new[] { @event });

      if (@event is CreatedEventBase created)
      {
        CreatedAt = created.OccurredAt;
        CreatedById = created.UserId;
      }
      else if (@event is DeletedEventBase deleted)
      {
        DeletedAt = deleted.OccurredAt;
        DeletedById = deleted.UserId;
      }
      else if (@event is UpdatedEventBase updated)
      {
        UpdatedAt = updated.OccurredAt;
        UpdatedById = updated.UserId;
      }

      Version++;
    }

    public override bool Equals(object? obj) => obj is Aggregate aggregate
      && aggregate.GetType().Equals(GetType())
      && aggregate.Id == Id;
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    public override string ToString() => $"{GetType()} (Id={Id})";
  }
}
