namespace Logitar.Portal.Domain
{
  public static class StringExtensions
  {
    public static string? CleanTrim(this string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    public static string Remove(this string s, string pattern) => s.Replace(pattern, string.Empty);

    public static string FromUriSafeHash(this string s)
    {
      if (s.Length % 4 > 0)
      {
        s = s.PadRight((s.Length / 4 + 1) * 4, '=');
      }

      return s.Replace('_', '/').Replace('-', '+');
    }
    public static string ToUriSafeHash(this string s) => s.Replace('+', '-').Replace('/', '_').TrimEnd('=');
  }
}
