using Portal.Core.ApiKeys.Payloads;

namespace Portal.Core.ApiKeys.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(string keyHash, CreateApiKeyPayload payload, Guid userId) : base(userId)
    {
      KeyHash = keyHash ?? throw new ArgumentNullException(nameof(keyHash));
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public string KeyHash { get; private set; }
    public CreateApiKeyPayload Payload { get; private set; }
  }
}
