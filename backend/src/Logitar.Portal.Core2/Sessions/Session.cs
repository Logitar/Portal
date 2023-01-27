using Logitar.Portal.Core2.Sessions.Events;
using Logitar.Portal.Core2.Users;

namespace Logitar.Portal.Core2.Sessions
{
  internal class Session : AggregateRoot
  {
    public Session(User user, string? keyHash = null, string? ipAddress = null, string? additionalInformation = null)
    {
      ApplyChange(new SessionCreatedEvent
      {
        KeyHash = keyHash,
        IpAddress = ipAddress,
        AdditionalInformation = additionalInformation
      }, user.Id);
    }
    private Session()
    {
    }

    public string? KeyHash { get; private set; }

    public string? IpAddress { get; private set; }
    public string? AdditionalInformation { get; private set; }

    protected virtual void Apply(SessionCreatedEvent @event)
    {
      KeyHash = KeyHash;

      IpAddress = IpAddress;
      AdditionalInformation = AdditionalInformation;
    }
  }
}
