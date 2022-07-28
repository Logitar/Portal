using System.Text.Json;

namespace Portal.Web
{
  internal static class ModelSerializer
  {
    public static string? Serialize<T>(T model) => JsonSerializer.Serialize(model, new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });
  }
}
