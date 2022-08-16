namespace Logitar.Portal.Domain.Sessions.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(Guid userId, string? keyHash = null, string? ipAddress = null, string? additionalInformation = null)
      : base(userId)
    {
      AdditionalInformation = additionalInformation;
      KeyHash = keyHash;
      IpAddress = ipAddress;
    }

    public string? AdditionalInformation { get; private set; }
    public string? KeyHash { get; private set; }
    public string? IpAddress { get; private set; }
  }
}
