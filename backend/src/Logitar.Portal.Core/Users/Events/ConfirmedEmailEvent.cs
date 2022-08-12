namespace Logitar.Portal.Core.Users.Events
{
  public class ConfirmedEmailEvent : EventBase
  {
    public ConfirmedEmailEvent(Guid userId) : base(userId)
    {
    }
  }
}
