using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Logitar.Portal.Client.Implementations
{
  internal class JsonContent : StringContent
  {
    public JsonContent(object? value, Encoding? encoding = null) : base(
      JsonSerializer.Serialize(value),
      encoding ?? Encoding.UTF8,
      MediaTypeNames.Application.Json
    )
    {
    }
  }
}
