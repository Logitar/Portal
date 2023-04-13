namespace Logitar.Portal.Contracts.Errors;

public record Error
{
  public string Code { get; set; } = string.Empty;
  public string? Description { get; set; }
  public IEnumerable<ErrorData> Data { get; set; } = Enumerable.Empty<ErrorData>();
}
