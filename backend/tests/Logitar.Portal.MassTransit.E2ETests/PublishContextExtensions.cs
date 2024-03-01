using MassTransit;

namespace Logitar.Portal.MassTransit;

internal static class PublishContextExtensions
{
  public static void Populate(this PublishContext context, Guid correlationId, IConfiguration configuration)
  {
    context.CorrelationId = correlationId;

    string? realm = configuration.GetValue<string>("Realm");
    if (!string.IsNullOrWhiteSpace(realm))
    {
      context.Headers.Set(Contracts.Constants.Headers.Realm, realm.Trim());
    }

    string? user = configuration.GetValue<string>("User");
    if (!string.IsNullOrWhiteSpace(user))
    {
      context.Headers.Set(Contracts.Constants.Headers.User, user.Trim());
    }
  }
}
