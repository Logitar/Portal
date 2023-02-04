using Logitar.Portal.Contracts.Messages;
using System.Globalization;

namespace Logitar.Portal.Application.Messages
{
  public record SendDemoMessagePayload
  {
    public string TemplateId { get; set; } = string.Empty;

    public CultureInfo? Locale { get; set; }

    public IEnumerable<VariablePayload>? Variables { get; set; }
  }
}
