using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Domain.Messages;

public record SenderSummary
{
  public SenderId Id { get; }
  public bool IsDefault { get; }
  public EmailUnit Email { get; }
  public DisplayNameUnit? DisplayName { get; }
  public SenderProvider Provider { get; }

  [JsonConstructor]
  public SenderSummary(SenderId id, bool isDefault, EmailUnit email, DisplayNameUnit? displayName, SenderProvider provider)
  {
    Id = id;
    IsDefault = isDefault;
    Email = email;
    DisplayName = displayName;
    Provider = provider;
  }

  public SenderSummary(SenderAggregate sender) : this(sender.Id, sender.IsDefault, sender.Email, sender.DisplayName, sender.Provider)
  {
  }
}
