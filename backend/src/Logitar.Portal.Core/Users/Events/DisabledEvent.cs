namespace Logitar.Portal.Core.Users.Events
{
  public class DisabledEvent : EventBase
  {
    public DisabledEvent(Guid userId) : base(userId)
    {
    }
  }
}
