using Logitar.Portal.Core.Sessions;
using System.Net;
using System.Text;

namespace Logitar.Portal.Core.Accounts
{
  internal class SessionNotBelongingToUserException : ApiException
  {
    public SessionNotBelongingToUserException(Session session, Guid userId)
      : base(HttpStatusCode.Forbidden, GetMessage(session, userId))
    {
      Session = session ?? throw new ArgumentNullException(nameof(session));
      UserId = userId;
    }

    public Session Session { get; }
    public Guid UserId { get; }

    private static string GetMessage(Session session, Guid userId)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified session does not belong to the specified user.");
      message.AppendLine($"Session: {session}");
      message.AppendLine($"UserID: {userId}");

      return message.ToString();
    }
  }
}
