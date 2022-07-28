using Portal.Core.ApiKeys.Payloads;

namespace Portal.Core.ApiKeys.Events
{
  public class UpdatedEvent : UpdatedEventBase
  {
    public UpdatedEvent(UpdateApiKeyPayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public UpdateApiKeyPayload Payload { get; private set; }
  }
}
