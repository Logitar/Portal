using Logitar.Portal.Infrastructure.JsonConverters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Web.Models
{
  internal static class ModelSerializer
  {
    private static readonly JsonSerializerOptions _options = new();

    static ModelSerializer()
    {
      _options.Converters.Add(new CultureInfoConverter());
      _options.Converters.Add(new JsonStringEnumConverter());
      _options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }

    public static string? Serialize<T>(T model) => JsonSerializer.Serialize(model, _options);
  }
}
