using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Messages
{
  public class DefaultSenderRequiredException : Exception
  {
    public DefaultSenderRequiredException(Realm? realm) : base(GetMessage(realm))
    {
      Data["Realm"] = realm == null ? "Portal" : realm.ToString();
    }

    private static string GetMessage(Realm? realm) => realm == null
      ? "The default Sender is required for the Portal."
      : $"The default Sender is required for the realm '{realm.Alias}'.";
  }
}
