namespace Logitar.Portal.Application.Logging;

public record Error
{
  public Error()
  {
    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public ErrorSeverity Severity { get; set; }
  public string Code { get; set; } = string.Empty;
  public string? Description { get; set; }

  protected JsonSerializerOptions SerializerOptions { get; } = new();

  public string Serialize() => JsonSerializer.Serialize(this, GetType(), SerializerOptions);
}
