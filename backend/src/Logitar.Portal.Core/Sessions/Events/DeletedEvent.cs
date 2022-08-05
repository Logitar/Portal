namespace Logitar.Portal.Core.Sessions.Events
{
  public class DeletedEvent : DeletedEventBase
  {
    public DeletedEvent(Guid userId) : base(userId)
    {
    }
  }
}
