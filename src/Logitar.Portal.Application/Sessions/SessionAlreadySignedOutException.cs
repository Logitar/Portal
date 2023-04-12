using Logitar.Portal.Core;
using Logitar.Portal.Domain.Sessions;
using System.Net;

namespace Logitar.Portal.Application.Sessions
{
  internal class SessionAlreadySignedOutException : ApiException
  {
    public SessionAlreadySignedOutException(Session session)
      : base(HttpStatusCode.BadRequest, $"The session '{session}' is already signed-out.")
    {
      Session = session ?? throw new ArgumentNullException(nameof(session));
      Value = new { code = nameof(SessionAlreadySignedOutException).Remove(nameof(Exception)) };
    }

    public Session Session { get; }
  }
}
