namespace Logitar.Portal.v2.Contracts.Errors;

public record Error
{
  public string Code { get; set; } = string.Empty;
  public string? Description { get; set; }
}
