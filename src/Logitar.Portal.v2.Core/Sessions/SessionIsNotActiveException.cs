namespace Logitar.Portal.v2.Core.Sessions;

public class SessionIsNotActiveException : Exception
{
  public SessionIsNotActiveException(SessionAggregate session)
    : base($"The session '{session}' is not active.")
  {
    Data["Session"] = session.ToString();
  }
}
