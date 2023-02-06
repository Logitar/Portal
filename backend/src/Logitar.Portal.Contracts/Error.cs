using System.Text.Json.Serialization;

namespace Logitar.Portal.Contracts
{
  public class Error
  {
    public Error(Exception exception)
      : this(exception.GetCode(), exception.Message, exception.GetData(), ErrorSeverity.Failure)
    {
    }

    [JsonConstructor]
    public Error(string code, string? description = null, IReadOnlyDictionary<string, string?>? data = null, ErrorSeverity severity2 = ErrorSeverity.Failure)
    {
      Severity = severity2;
      Code = code;
      Description = description;
      Data = data;
    }

    public ErrorSeverity Severity { get; }
    public string Code { get; }
    public string? Description { get; }
    public IReadOnlyDictionary<string, string?>? Data { get; }

    public override string ToString() => Description == null ? Code : $"{Code}: {Description}";
  }
}
