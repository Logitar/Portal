namespace Logitar.Portal.Domain.Users.Events
{
  public class ConfirmedEmailEvent : EventBase
  {
    public ConfirmedEmailEvent(Guid userId) : base(userId)
    {
    }
  }
}
