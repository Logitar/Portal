using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Accounts
{
  public class GoogleClientIdRequiredException : Exception
  {
    public GoogleClientIdRequiredException(Realm realm)
      : base($"The realm '{realm}' does not have a configured Google Client ID.")
    {
      Data["Realm"] = realm.ToString();
    }
  }
}
