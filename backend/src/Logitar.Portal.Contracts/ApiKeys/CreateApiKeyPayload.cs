using System;

namespace Logitar.Portal.Contracts.ApiKeys
{
  public record CreateApiKeyPayload
  {
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime? ExpiresOn { get; set; }
  }
}
