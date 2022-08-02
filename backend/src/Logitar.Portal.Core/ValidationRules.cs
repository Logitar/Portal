namespace Logitar.Portal.Core
{
  internal static class ValidationRules
  {
    public static bool BeAValidIdentifier(string? value) => string.IsNullOrEmpty(value)
      || (!char.IsDigit(value.First()) && value.All(c => char.IsLetterOrDigit(c) || c == '_'));
    
    public static bool BeAValidUrl(string? s) => s == null || Uri.IsWellFormedUriString(s, UriKind.Absolute);
  }
}
