namespace Logitar.Portal.Core.Emails.Templates.Events
{
  public class DeletedEvent : DeletedEventBase
  {
    public DeletedEvent(Guid userId) : base(userId)
    {
    }
  }
}
