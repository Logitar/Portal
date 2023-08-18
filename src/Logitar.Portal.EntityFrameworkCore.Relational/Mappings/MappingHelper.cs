using Logitar.Portal.Contracts;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Mappings;

internal static class MappingHelper
{
  public static IEnumerable<CustomAttribute> GetCustomAttributes(Dictionary<string, string> customAttributes)
    => customAttributes.Select(customAttribute => new CustomAttribute(customAttribute.Key, customAttribute.Value));

  public static DateTime ToUtcDateTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
  public static DateTime? ToUtcDateTime(DateTime? value)
    => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
}
