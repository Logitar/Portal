using Portal.Core.Realms.Payloads;

namespace Portal.Core.Realms.Events
{
  public class UpdatedEvent : UpdatedEventBase
  {
    public UpdatedEvent(UpdateRealmPayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public UpdateRealmPayload Payload { get; private set; }
  }
}
