namespace Logitar.Portal.EntityFrameworkCore.Relational.Mappings;

internal static class MappingExtensions
{
  public static DateTime AsUtc(this DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
  public static DateTime? AsUtc(this DateTime? value)
    => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
}
