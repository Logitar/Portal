using System.Net;

namespace Portal.Core.Accounts
{
  internal class InvalidCredentialsException : ApiException
  {
    public const string Code = "InvalidCredentials";

    public InvalidCredentialsException()
      : base(HttpStatusCode.BadRequest, "The specified credentials did not match.")
    {
      Value = new { Code };
    }
  }
}
