using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Contracts;

public abstract class AggregateModel
{
  public Guid Id { get; set; }
  public long Version { get; set; }

  public ActorModel CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public ActorModel UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public override bool Equals(object? obj) => obj is AggregateModel aggregate && aggregate.GetType().Equals(GetType()) && aggregate.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString() => $"{GetType()} (Id={Id})";
}
