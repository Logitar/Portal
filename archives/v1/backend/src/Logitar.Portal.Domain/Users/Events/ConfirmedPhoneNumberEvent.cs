namespace Logitar.Portal.Domain.Users.Events
{
  public class ConfirmedPhoneNumberEvent : EventBase
  {
    public ConfirmedPhoneNumberEvent(Guid userId) : base(userId)
    {
    }
  }
}
