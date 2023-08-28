namespace Logitar.Portal.Web.Constants;

internal static class Schemes
{
  public const string Basic = nameof(Basic);
  public const string Session = nameof(Session);

  public static readonly string[] All = new[] { Basic, Session };
}
