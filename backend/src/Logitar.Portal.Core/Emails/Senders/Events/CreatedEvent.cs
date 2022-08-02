using Logitar.Portal.Core.Emails.Senders.Payloads;

namespace Logitar.Portal.Core.Emails.Senders.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(bool isDefault, CreateSenderPayload payload, Guid userId) : base(userId)
    {
      IsDefault = isDefault;
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public bool IsDefault { get; private set; }
    public CreateSenderPayload Payload { get; private set; }
  }
}
