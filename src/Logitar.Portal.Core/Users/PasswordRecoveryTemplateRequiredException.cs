using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Users;

public class PasswordRecoveryTemplateRequiredException : Exception
{
  public PasswordRecoveryTemplateRequiredException(RealmAggregate realm)
    : base($"The realm '{realm}' does not have a configured password recovery template.")
  {
    Data["Realm"] = realm.ToString();
  }
}
