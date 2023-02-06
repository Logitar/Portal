using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Web.Models
{
  internal static class ModelSerializer
  {
    private static readonly JsonSerializerOptions _serializerOptions = new();

    static ModelSerializer()
    {
      _serializerOptions.Converters.Add(new JsonStringEnumConverter());
      _serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }

    public static string? Serialize<T>(T model) => JsonSerializer.Serialize(model, _serializerOptions);
  }
}
