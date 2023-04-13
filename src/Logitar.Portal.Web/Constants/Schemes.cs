using SharedSchemes = Logitar.Portal.Contracts.Constants.Schemes;

namespace Logitar.Portal.Web.Constants;

internal static class Schemes
{
  public const string Session = nameof(Session);

  public static string[] All => new[] { SharedSchemes.Basic, Session };
}
