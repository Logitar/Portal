using Portal.Core.Users.Payloads;

namespace Portal.Core.Users.Events
{
  public class UpdatedEvent : UpdatedEventBase
  {
    public UpdatedEvent(UpdateUserPayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public UpdateUserPayload Payload { get; private set; }
  }
}
