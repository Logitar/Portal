using System.Net;

namespace Logitar.Portal.Core.Emails.Messages
{
  internal class UserEmailRequiredException : ApiException
  {
    public UserEmailRequiredException(Guid id)
      : base(HttpStatusCode.BadRequest, $"The user email is required.{Environment.NewLine}User ID: {id}")
    {
      Id = id;
      Value = new { code = nameof(UserEmailRequiredException).Remove(nameof(Exception)) };
    }

    public Guid Id { get; }
  }
}
