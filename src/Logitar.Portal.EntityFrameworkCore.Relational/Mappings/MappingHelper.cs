namespace Logitar.Portal.EntityFrameworkCore.Relational.Mappings;

internal static class MappingHelper
{
  public static DateTime AsUtc(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
