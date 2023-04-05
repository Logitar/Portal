namespace Logitar.Portal.Domain.Users.Events
{
  public class DisabledEvent : EventBase
  {
    public DisabledEvent(Guid userId) : base(userId)
    {
    }
  }
}
