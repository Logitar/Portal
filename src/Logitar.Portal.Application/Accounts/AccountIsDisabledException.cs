using Logitar.Portal.Core;
using System.Net;

namespace Logitar.Portal.Application.Accounts
{
  internal class AccountIsDisabledException : ApiException
  {
    public AccountIsDisabledException(Guid userId)
      : base(HttpStatusCode.BadRequest, $"The user 'Id={userId}' is disabled.")
    {
      UserId = userId;
      Value = new { code = nameof(AccountIsDisabledException).Remove(nameof(Exception)) };
    }

    public Guid UserId { get; }
  }
}
