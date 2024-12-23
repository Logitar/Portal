using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Domain.Messages;

public record SenderSummary
{
  public SenderId Id { get; }
  public bool IsDefault { get; }
  public Email? Email { get; }
  public Phone? Phone { get; }
  public DisplayName? DisplayName { get; }
  public SenderProvider Provider { get; }

  public SenderSummary(SenderId id, bool isDefault, Email email, DisplayName? displayName, SenderProvider provider)
    : this(id, isDefault, email, phone: null, displayName, provider)
  {
  }
  [JsonConstructor]
  public SenderSummary(SenderId id, bool isDefault, Email? email, Phone? phone, DisplayName? displayName, SenderProvider provider)
  {
    Id = id;
    IsDefault = isDefault;
    Email = email;
    Phone = phone;
    DisplayName = displayName;
    Provider = provider;
  }

  public SenderSummary(SenderAggregate sender) : this(sender.Id, sender.IsDefault, sender.Email, sender.Phone, sender.DisplayName, sender.Provider)
  {
  }

  public MailAddress ToMailAddress() // ISSUE #467: move to Logitar.Portal.Infrastructure.Messages.MessageExtensions and remove System usings
  {
    if (Email == null)
    {
      throw new InvalidOperationException($"The sender must be an {nameof(SenderType.Email)} sender in order to be converted into a {nameof(MailAddress)}.");
    }
    return new(Email.Address, DisplayName?.Value);
  }
}
