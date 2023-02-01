using System.Diagnostics.CodeAnalysis;

namespace Logitar.Portal.Domain
{
  public readonly struct AggregateId
  {
    public AggregateId(Guid value) : this(value.ToHash())
    {
    }
    public AggregateId(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException("The value is required.", nameof(value));
      }

      Value = value;
    }

    public string Value { get; }

    public static AggregateId NewId() => new(Guid.NewGuid());

    public static bool operator ==(AggregateId x, AggregateId y) => x.Equals(y);
    public static bool operator !=(AggregateId x, AggregateId y) => !x.Equals(y);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is AggregateId id && id.Value == Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
  }
}
