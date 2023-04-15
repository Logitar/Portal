using Logitar.Portal.Core.Users.Payloads;

namespace Logitar.Portal.Domain.Users.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(CreateUserSecurePayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public CreateUserSecurePayload Payload { get; private set; }
  }
}
