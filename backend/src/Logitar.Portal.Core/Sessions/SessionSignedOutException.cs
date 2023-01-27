namespace Logitar.Portal.Core.Sessions
{
  internal class SessionSignedOutException : Exception
  {
    public SessionSignedOutException(Session session)
      : base($"The session '{session}' is signed-out.")
    {
      Data["Session"] = session.ToString();
    }
  }
}
