namespace Logitar.Portal.Contracts.Sessions;

public record RenewSessionPayload
{
  public string RefreshToken { get; set; } = string.Empty;

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
