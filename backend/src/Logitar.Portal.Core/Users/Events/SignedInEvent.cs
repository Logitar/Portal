namespace Logitar.Portal.Core.Users.Events
{
  public class SignedInEvent : EventBase
  {
    public SignedInEvent(DateTime signedInAt, Guid userId) : base(userId)
    {
      OccurredAt = signedInAt;
    }
  }
}
