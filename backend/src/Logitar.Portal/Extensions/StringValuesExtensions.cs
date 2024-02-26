using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Extensions;

internal static class StringValuesExtensions
{
  public static IReadOnlyCollection<string> ExtractValues(this StringValues values)
  {
    List<string> nonEmptyValues = new(capacity: values.Count);

    foreach (string? value in values)
    {
      if (!string.IsNullOrWhiteSpace(value))
      {
        nonEmptyValues.Add(value.Trim());
      }
    }

    return nonEmptyValues.AsReadOnly();
  }
}
