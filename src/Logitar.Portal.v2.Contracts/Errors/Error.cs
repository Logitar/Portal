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

  public static Error From(Exception exception, ErrorSeverity severity = ErrorSeverity.Failure)
  {
    List<ErrorData> data = new(exception.Data.Count);
    foreach (object key in exception.Data.Keys)
    {
      object? value = exception.Data[key];
      if (value != null)
      {
        data.Add(new ErrorData
        {
          Key = JsonSerializer.Serialize(key, _options),
          Value = JsonSerializer.Serialize(value, _options)
        });
      }
    }

    return new Error
    {
      Severity = severity,
      Code = exception.GetType().Name.Replace(nameof(Exception), string.Empty),
      Description = exception.Message,
      Data = data
    };
  }

  public string Serialize() => JsonSerializer.Serialize(this, GetType(), _options);
}
