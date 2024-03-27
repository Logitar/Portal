using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Domain.Messages;

public record SenderSummary
{
  public SenderId Id { get; }
  public bool IsDefault { get; }
  public EmailUnit? Email { get; }
  public PhoneUnit? Phone { get; }
  public DisplayNameUnit? DisplayName { get; }
  public SenderProvider Provider { get; }

  public SenderSummary(SenderId id, bool isDefault, EmailUnit email, DisplayNameUnit? displayName, SenderProvider provider)
    : this(id, isDefault, email, phone: null, displayName, provider)
  {
  }
  [JsonConstructor]
  public SenderSummary(SenderId id, bool isDefault, EmailUnit? email, PhoneUnit? phone, DisplayNameUnit? displayName, SenderProvider provider)
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
      throw new InvalidOperationException($"The sender must be an {nameof(SenderType.Email)} sender in order to be converted to a {nameof(MailAddress)}.");
    }
    return new(Email.Address, DisplayName?.Value);
  }
}
