using Logitar.Portal.Core.Dictionaries.Payloads;

namespace Logitar.Portal.Domain.Dictionaries.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(CreateDictionaryPayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public CreateDictionaryPayload Payload { get; private set; }
  }
}
