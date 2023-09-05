using Logitar.Portal.Contracts.Constants;

namespace Logitar.Portal.Web.Constants;

internal static class WebSchemes
{
  public const string Session = nameof(Session);

  public static readonly string[] All = new[] { Schemes.ApiKey, Schemes.Basic, Session };
}
