using Portal.Core.Templates.Payloads;

namespace Portal.Core.Templates.Events
{
  public class UpdatedEvent : UpdatedEventBase
  {
    public UpdatedEvent(UpdateTemplatePayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public UpdateTemplatePayload Payload { get; private set; }
  }
}
