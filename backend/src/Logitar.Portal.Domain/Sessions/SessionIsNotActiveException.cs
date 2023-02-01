namespace Logitar.Portal.Domain.Sessions
{
  public class SessionIsNotActiveException : Exception
  {
    public SessionIsNotActiveException(Session session)
      : base($"The session '{session}' is not active.")
    {
      Data["Session"] = session.ToString();
    }
  }
}
