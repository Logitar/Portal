using Portal.Core.Realms;
using System.Net;

namespace Portal.Core.Emails.Messages
{
  internal class DefaultSenderRequiredException : ApiException
  {
    public DefaultSenderRequiredException(Realm? realm)
      : base(HttpStatusCode.BadRequest, GetMessage(realm))
    {
      Realm = realm;

      Value = new { code = nameof(SenderNotInRealmException).Remove(nameof(Exception)) };
    }

    public Realm? Realm { get; }

    private static string GetMessage(Realm? realm) => realm == null
      ? "The default Sender is required for the Portal."
      : $"The default Sender is required for the realm '{realm.Alias}'.";
  }
}
