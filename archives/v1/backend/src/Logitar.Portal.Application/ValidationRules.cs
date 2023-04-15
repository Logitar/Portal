using System.Globalization;

namespace Logitar.Portal.Application
{
  internal static class ValidationRules
  {
    public static bool BeAValidCulture(string? value)
    {
      try
      {
        return value == null || CultureInfo.GetCultureInfo(value).LCID != 4096;
      }
      catch (CultureNotFoundException)
      {
        return false;
      }
    }

    public static bool BeAValidIdentifier(string? value) => string.IsNullOrEmpty(value)
      || (!char.IsDigit(value.First()) && value.All(c => char.IsLetterOrDigit(c) || c == '_'));

    public static bool BeAValidUrl(string? s) => s == null || Uri.IsWellFormedUriString(s, UriKind.Absolute);
  }
}
