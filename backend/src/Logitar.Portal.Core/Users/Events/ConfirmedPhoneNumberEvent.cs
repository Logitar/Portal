namespace Logitar.Portal.Core.Users.Events
{
  public class ConfirmedPhoneNumberEvent : EventBase
  {
    public ConfirmedPhoneNumberEvent(Guid userId) : base(userId)
    {
    }
  }
}
