using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders;

public abstract record SenderSettings
{
  [JsonIgnore]
  public abstract SenderProvider Provider { get; }
}
