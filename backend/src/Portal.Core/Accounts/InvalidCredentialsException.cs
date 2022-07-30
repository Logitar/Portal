using System.Net;

namespace Portal.Core.Accounts
{
  internal class InvalidCredentialsException : ApiException
  {
    public InvalidCredentialsException()
      : base(HttpStatusCode.BadRequest, "The specified credentials did not match.")
    {
      Value = new { code = nameof(InvalidCredentialsException).Remove(nameof(Exception)) };
    }
  }
}
