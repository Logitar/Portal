using Logitar.Security.Claims;

namespace Logitar.Portal.Domain.Users;

public record Gender
{
  public const int MaximumLength = byte.MaxValue;

  public Gender(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentException("The gender value is required.", nameof(value));
    }

    value = value.Trim();
    if (value.Length > MaximumLength)
    {
      throw new ArgumentOutOfRangeException(nameof(value), $"The value cannot exceed {MaximumLength} characters.");
    }

    Value = Format(value);
  }

  public string Value { get; }

  public static IImmutableSet<string> KnownValues { get; } = ImmutableHashSet.Create(new[] { Genders.Female, Genders.Male });
  public static string Format(string value) => KnownValues.Contains(value.ToLower()) ? value.ToLower() : value;
}
