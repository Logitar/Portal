namespace Logitar.Portal.Contracts.Sessions;

public record RenewPayload
{
  public string RefreshToken { get; set; } = string.Empty;

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
