namespace Logitar.Portal.Web.Constants;

internal static class Schemes
{
  public const string ApiKey = nameof(ApiKey);
  public const string Basic = nameof(Basic);
  public const string Session = nameof(Session);

  public static readonly string[] All = new[] { ApiKey, Basic, Session };
}
