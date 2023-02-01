using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Sessions
{
  public class Session : AggregateRoot
  {
    public Session(User user, string? keyHash, string? ipAddress, string? additionalInformation) : base()
    {
      ApplyChange(new SessionCreatedEvent
      {
        KeyHash = keyHash,
        IpAddress = ipAddress?.CleanTrim(),
        AdditionalInformation = additionalInformation?.CleanTrim()
      }, user.Id);
    }
    private Session() : base()
    {
    }

    public string? KeyHash { get; private set; }

    public string? IpAddress { get; private set; }
    public string? AdditionalInformation { get; private set; }

    protected virtual void Apply(SessionCreatedEvent @event)
    {
      KeyHash = @event.KeyHash;

      IpAddress = @event.IpAddress;
      AdditionalInformation = @event.AdditionalInformation;
    }
  }
}
