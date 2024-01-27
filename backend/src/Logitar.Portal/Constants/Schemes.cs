namespace Logitar.Portal.Constants;

internal static class Schemes
{
  public const string ApiKey = nameof(ApiKey);
  public const string Basic = nameof(Basic);
  public const string Session = nameof(Session);

  public static readonly IEnumerable<string> All = [ApiKey, Basic, Session];
}
