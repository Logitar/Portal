namespace Logitar.Portal.Domain.Users.Events
{
  public class EnabledEvent : EventBase
  {
    public EnabledEvent(Guid userId) : base(userId)
    {
    }
  }
}
