using System.Collections.Generic;

namespace Logitar.Portal.Contracts.Senders
{
  public record UpdateSenderPayload
  {
    public string EmailAddress { get; set; } = string.Empty;
    public string? DisplayName { get; set; }

    public IEnumerable<SettingPayload>? Settings { get; set; }
  }
}
