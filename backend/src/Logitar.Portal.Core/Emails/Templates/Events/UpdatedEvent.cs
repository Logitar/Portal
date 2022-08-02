using Logitar.Portal.Core.Emails.Templates.Payloads;

namespace Logitar.Portal.Core.Emails.Templates.Events
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
