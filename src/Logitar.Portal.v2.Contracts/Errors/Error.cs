using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.v2.Contracts.Errors;

public record Error
{
  private static readonly JsonSerializerOptions _options = new();
  static Error() => _options.Converters.Add(new JsonStringEnumConverter());

  public ErrorSeverity Severity { get; set; }
  public string Code { get; set; } = string.Empty;
  public string? Description { get; set; }
  public IEnumerable<ErrorData> Data { get; set; } = Enumerable.Empty<ErrorData>();

  public string Serialize() => JsonSerializer.Serialize(this, GetType(), _options);
}
