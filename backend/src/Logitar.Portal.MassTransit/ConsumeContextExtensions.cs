using Logitar.Portal.Application;
using MassTransit;

namespace Logitar.Portal.MassTransit;

internal static class ConsumeContextExtensions
{
  public static ContextParameters GetParameters(this ConsumeContext context)
  {
    ContextParameters parameters = new();

    if (context.TryGetHeader(Contracts.Constants.Headers.Realm, out string? realm))
    {
      parameters.RealmOverride = realm;
    }
    if (context.TryGetHeader(Contracts.Constants.Headers.User, out string? user))
    {
      parameters.ImpersonifiedUser = user;
    }

    return parameters;
  }
}
