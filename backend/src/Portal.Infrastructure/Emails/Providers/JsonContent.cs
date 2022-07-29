using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Portal.Infrastructure.Emails.Providers
{
  internal class JsonContent : StringContent
  {
    private static readonly JsonSerializerOptions _options = new();

    static JsonContent()
    {
      _options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }

    public JsonContent(object? value) : base(
      JsonSerializer.Serialize(value, _options),
      Encoding.UTF8,
      MediaTypeNames.Application.Json
    )
    {
    }
  }
}
