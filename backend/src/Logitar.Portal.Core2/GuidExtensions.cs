namespace Logitar.Portal.Core2
{
  internal static class GuidExtensions
  {
    public static Guid FromHash(this string hash) => new(Convert.FromBase64String(string.Concat(hash, "==")
      .Replace('-', '+')
      .Replace('_', '/')));

    public static string ToHash(this Guid guid) => Convert.ToBase64String(guid.ToByteArray())
      .Replace('+', '-')
      .Replace('/', '_')
      .Remove("=");
  }
}
