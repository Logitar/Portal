namespace Logitar.Portal.Domain.Sessions
{
  public class SessionAlreadySignedOutException : Exception
  {
    public SessionAlreadySignedOutException(Session session)
      : base($"The session '{session}' is already signed out.")
    {
      Data["Session"] = session.ToString();
    }
  }
}
