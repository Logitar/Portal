using System.Net;

namespace Portal.Core.Accounts
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
