using Portal.Core.Emails.Templates.Payloads;

namespace Portal.Core.Emails.Templates.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(CreateTemplatePayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public CreateTemplatePayload Payload { get; private set; }
  }
}
