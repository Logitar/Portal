using Logitar.Portal.Core;
using System.Net;

namespace Logitar.Portal.Application.Accounts
{
  internal class InvalidCredentialsException : ApiException
  {
    public InvalidCredentialsException(Exception? innerException = null)
      : base(HttpStatusCode.BadRequest, "The specified credentials did not match.", innerException)
    {
      Value = new { code = nameof(InvalidCredentialsException).Remove(nameof(Exception)) };
    }
  }
}
