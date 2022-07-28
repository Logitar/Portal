using Portal.Core.Users.Payloads;

namespace Portal.Core.Users.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(CreateUserPayload payload, Guid userId, string? passwordHash) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
      PasswordHash = passwordHash;
    }

    public string? PasswordHash { get; private set; }
    public CreateUserPayload Payload { get; private set; }
  }
}
