using System;

namespace Logitar.Portal.Contracts.ApiKeys
{
  public record ApiKeyModel : AggregateModel
  {
    public string? XApiKey { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime? ExpiresOn { get; set; }
  }
}
