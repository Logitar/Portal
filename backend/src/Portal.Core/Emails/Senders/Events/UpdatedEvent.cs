using Portal.Core.Emails.Senders.Payloads;

namespace Portal.Core.Emails.Senders.Events
{
  public class UpdatedEvent : UpdatedEventBase
  {
    public UpdatedEvent(UpdateSenderPayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public UpdateSenderPayload Payload { get; private set; }
  }
}
