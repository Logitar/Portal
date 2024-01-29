namespace Logitar.Portal.Contracts.Sessions;

public record RenewSessionPayload
{
  public string RefreshToken { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public RenewSessionPayload() : this(string.Empty)
  {
  }

  public RenewSessionPayload(string refreshToken)
  {
    RefreshToken = refreshToken;
    CustomAttributes = [];
  }

  public RenewSessionPayload(string refreshToken, IEnumerable<CustomAttribute> customAttributes) : this(refreshToken)
  {
    CustomAttributes.AddRange(customAttributes);
  }
}
