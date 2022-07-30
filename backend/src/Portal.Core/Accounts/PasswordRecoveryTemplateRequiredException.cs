using Portal.Core.Realms;
using System.Net;

namespace Portal.Core.Accounts
{
  internal class PasswordRecoveryTemplateRequiredException : ApiException
  {
    public PasswordRecoveryTemplateRequiredException(Realm realm)
      : base(HttpStatusCode.BadRequest, $"The password recovery template is required for the realm '{realm}'.")
    {
      Realm = realm ?? throw new ArgumentNullException(nameof(realm));
      Value = new { code = nameof(PasswordRecoveryTemplateRequiredException).Remove(nameof(Exception)) };
    }

    public Realm Realm { get; }
  }
}
