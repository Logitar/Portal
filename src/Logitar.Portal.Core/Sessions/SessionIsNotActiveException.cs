namespace Logitar.Portal.Core.Sessions;

public class SessionIsNotActiveException : Exception
{
  public SessionIsNotActiveException(SessionAggregate session)
    : base($"The session '{session}' is not active.")
  {
    Data["Session"] = session.ToString();
  }
}
