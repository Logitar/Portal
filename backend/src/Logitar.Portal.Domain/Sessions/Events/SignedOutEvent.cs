namespace Logitar.Portal.Domain.Sessions.Events
{
  public class SignedOutEvent : EventBase
  {
    public SignedOutEvent(Guid userId) : base(userId)
    {
    }
  }
}
