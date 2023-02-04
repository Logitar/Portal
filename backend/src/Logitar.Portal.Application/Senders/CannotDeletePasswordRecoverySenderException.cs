using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders
{
  public class CannotDeletePasswordRecoverySenderException : Exception
  {
    public CannotDeletePasswordRecoverySenderException(Sender sender, Realm realm)
      : base($"The password recovery sender '{sender}' from realm '{realm}' cannot be deleted.")
    {
      Data["Sender"] = sender.ToString();
      Data["Realm"] = realm.ToString();
    }
  }
}
