namespace Logitar.Portal.v2.Contracts.Sessions;

public record RefreshInput
{
  public string RefreshToken { get; set; } = string.Empty;

  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }
}
