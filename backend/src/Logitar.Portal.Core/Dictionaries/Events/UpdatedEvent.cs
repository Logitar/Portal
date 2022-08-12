using Logitar.Portal.Core.Dictionaries.Payloads;

namespace Logitar.Portal.Core.Dictionaries.Events
{
  public class UpdatedEvent : UpdatedEventBase
  {
    public UpdatedEvent(UpdateDictionaryPayload payload, Guid userId) : base(userId)
    {
      Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public UpdateDictionaryPayload Payload { get; private set; }
  }
}
