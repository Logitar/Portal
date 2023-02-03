using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Templates
{
  public class CannotDeletePasswordRecoveryTemplateException : Exception
  {
    public CannotDeletePasswordRecoveryTemplateException(Template template, Realm realm)
      : base($"The password recovery template '{template}' from realm '{realm}' cannot be deleted.")
    {
      Data["Template"] = template.ToString();
      Data["Realm"] = realm.ToString();
    }
  }
}
