using System.Reflection;

namespace Logitar.Portal.Core
{
  public abstract class AggregateRoot
  {
    protected AggregateRoot() : this(AggregateId.NewId())
    {
    }
    protected AggregateRoot(AggregateId id)
    {
      Id = id;
    }

    public AggregateId Id { get; private set; }
    public long Version { get; private set; }
    public bool IsDeleted { get; protected set; }

    private readonly List<DomainEvent> _changes = new();
    public bool HasChanges => _changes.Any();
    public IReadOnlyCollection<DomainEvent> Changes => _changes.AsReadOnly();
    public void ClearChanges() => _changes.Clear();

    public static T LoadFromHistory<T>(IEnumerable<DomainEvent> history, AggregateId id) where T : AggregateRoot
    {
      ConstructorInfo constructor = typeof(T).GetTypeInfo()
        .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Array.Empty<Type>())
        ?? throw new MissingMethodException(typeof(T).GetName(), "ctor()");

      T aggregate = (T)constructor.Invoke(null)
        ?? throw new InvalidOperationException("The aggregate instance cannot be null.");
      aggregate.Id = id;

      foreach (DomainEvent change in history.OrderBy(x => x.Version))
      {
        aggregate.Dispatch(change);
      }

      return aggregate;
    }

    protected void ApplyChange(DomainEvent change, AggregateId userId)
    {
      change.AggregateId = Id;
      change.OccurredOn = DateTime.UtcNow;
      change.UserId = userId;
      change.Version = Version + 1;

      Dispatch(change);

      _changes.Add(change);
    }
    private void Dispatch(DomainEvent @event)
    {
      Type aggregateType = GetType();
      Type eventType = @event.GetType();

      MethodInfo method = aggregateType.GetTypeInfo()
        .GetMethod("Apply", BindingFlags.Instance | BindingFlags.NonPublic, new[] { eventType })
        ?? throw new EventNotSupportedException(aggregateType, eventType);

      method.Invoke(this, new[] { @event });

      Version = @event.Version;
    }

    public override bool Equals(object? obj) => obj is AggregateRoot aggregate
      && aggregate.GetType().Equals(GetType())
      && aggregate.Id == Id;
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    public override string ToString() => $"{GetType()} (Id={Id})";
  }
}
