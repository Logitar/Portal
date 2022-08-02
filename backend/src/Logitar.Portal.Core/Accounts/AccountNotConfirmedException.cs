using System.Net;

namespace Logitar.Portal.Core.Accounts
{
  internal class AccountNotConfirmedException : ApiException
  {
    public AccountNotConfirmedException(Guid userId)
      : base(HttpStatusCode.BadRequest, $"The user 'Id={userId}' does not have a confirmed account.")
    {
      UserId = userId;
      Value = new { code = nameof(AccountNotConfirmedException).Remove(nameof(Exception)) };
    }

    public Guid UserId { get; }
  }
}
