using NodaTime;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Portal.v2.Core.Users;

public readonly struct TimeZoneEntry
{
  private readonly DateTimeZone? _dateTimeZone;

  public TimeZoneEntry(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      throw new ArgumentException("The time zone ID is required.", nameof(id));
    }

    _dateTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(id)
      ?? throw new ArgumentOutOfRangeException(nameof(id));
  }

  public string Id => _dateTimeZone?.Id ?? string.Empty;

  public static bool operator ==(TimeZoneEntry left, TimeZoneEntry right) => left.Equals(right);
  public static bool operator !=(TimeZoneEntry left, TimeZoneEntry right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is TimeZoneEntry entry
    && entry._dateTimeZone?.Equals(_dateTimeZone) == true;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => Id;
}
