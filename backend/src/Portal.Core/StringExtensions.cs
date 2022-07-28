namespace Portal.Core
{
  internal static class StringExtensions
  {
    public static string? CleanTrim(this string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
  }
}
