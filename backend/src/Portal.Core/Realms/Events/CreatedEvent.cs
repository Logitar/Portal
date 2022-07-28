using Portal.Core.Realms.Payloads;

namespace Portal.Core.Realms.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(CreateRealmPayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public CreateRealmPayload Payload { get; private set; }
  }
}
