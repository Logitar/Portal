namespace Logitar.Portal;

internal static class Assertions
{
  public static void Equal(DateTime? expected, DateTime? actual, TimeSpan? precision = null)
  {
    if (expected == null || actual == null)
    {
      Assert.Null(expected);
      Assert.Null(actual);
      return;
    }

    Equal(expected.Value, actual.Value, precision);
  }
  public static void Equal(DateTime expected, DateTime actual, TimeSpan? precision = null)
  {
    if (expected.Kind == DateTimeKind.Unspecified)
    {
      expected = DateTime.SpecifyKind(expected, DateTimeKind.Utc);
    }
    if (actual.Kind == DateTimeKind.Unspecified)
    {
      actual = DateTime.SpecifyKind(actual, DateTimeKind.Utc);
    }

    if (precision.HasValue)
    {
      Assert.Equal(expected.ToUniversalTime(), actual.ToUniversalTime(), precision.Value);
    }
    else
    {
      Assert.Equal(expected.ToUniversalTime(), actual.ToUniversalTime());
    }
  }
}
