using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Accounts
{
  public class PasswordRecoveryTemplateRequiredException : Exception
  {
    public PasswordRecoveryTemplateRequiredException(Realm realm)
      : base($"The realm '{realm}' does not have a configured password recovery template.")
    {
      Data["Realm"] = realm.ToString();
    }
  }
}
