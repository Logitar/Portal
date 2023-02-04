using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Accounts
{
  internal static class UserExtensions
  {
    public static void EnsureIsTrusted(this User user, Realm? realm = null)
    {
      if (realm?.RequireConfirmedAccount == true && !user.IsAccountConfirmed)
      {
        throw new AccountNotConfirmedException(user);
      }
      else if (user.IsDisabled)
      {
        throw new AccountIsDisabledException(user);
      }
    }
  }
}
