namespace Logitar.Portal.Contracts.Sessions;

public record CreateSessionInput
{
  public Guid UserId { get; set; }

  public string? IpAddress { get; set; }
  public string? AdditionalInformation { get; set; }

  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }
}
