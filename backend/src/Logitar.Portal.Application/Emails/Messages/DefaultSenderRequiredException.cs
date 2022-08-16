using Logitar.Portal.Core;
using Logitar.Portal.Domain.Realms;
using System.Net;

namespace Logitar.Portal.Application.Emails.Messages
{
  internal class DefaultSenderRequiredException : ApiException
  {
    public DefaultSenderRequiredException(Realm? realm)
      : base(HttpStatusCode.BadRequest, GetMessage(realm))
    {
      Realm = realm;

      Value = new { code = nameof(DefaultSenderRequiredException).Remove(nameof(Exception)) };
    }

    public Realm? Realm { get; }

    private static string GetMessage(Realm? realm) => realm == null
      ? "The default Sender is required for the Logitar.Portal."
      : $"The default Sender is required for the realm '{realm.Alias}'.";
  }
}
