using System.Text.Json.Serialization;

namespace Logitar.Portal.Contracts
{
  public class Error
  {
    public Error(Exception exception, IReadOnlyDictionary<string, string?>? data = null)
      : this(exception?.GetType().Name.Replace(nameof(Exception), string.Empty) ?? string.Empty, exception?.Message, data)
    {
    }

    [JsonConstructor]
    public Error(string code, string? description = null, IReadOnlyDictionary<string, string?>? data = null)
    {
      Code = code;
      Description = description;
      Data = data;
    }

    public string Code { get; }
    public string? Description { get; }

    public IReadOnlyDictionary<string, string?>? Data { get; }

    public override string ToString() => Description == null ? Code : $"{Code}: {Description}";
  }
}
