namespace Logitar.Portal.Domain.Emails.Messages.Events
{
  public class SucceededEvent : UpdatedEventBase
  {
    public SucceededEvent(SendMessageResult result, Guid userId) : base(userId)
    {
      Result = result ?? throw new ArgumentNullException(nameof(result));
    }

    public SendMessageResult Result { get; private set; }
  }
}
