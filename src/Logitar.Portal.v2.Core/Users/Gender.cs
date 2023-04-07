using System.Diagnostics.CodeAnalysis;

namespace Logitar.Portal.v2.Core.Users;

public readonly struct Gender
{
  private readonly string? _value;

  public Gender(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("The gender value is required.", nameof(value));
    }

    value = value.Trim();

    _value = value.ToLower() switch
    {
      "female" => nameof(Female),
      "male" => nameof(Male),
      "other" => nameof(Other),
      _ => value,
    };
  }

  public static Gender Female { get; } = new(nameof(Female));
  public static Gender Male { get; } = new(nameof(Male));
  public static Gender Other { get; } = new(nameof(Other));

  public string Value => _value ?? string.Empty;

  public static bool operator ==(Gender left, Gender right) => left.Equals(right);
  public static bool operator !=(Gender left, Gender right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is Gender gender && gender._value == _value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
