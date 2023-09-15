using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages.Validators;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Messages;

public record ReadOnlyRecipient
{
  public ReadOnlyRecipient(string address, string? displayName = null, RecipientType type = RecipientType.To, AggregateId? userId = null)
  {
    Type = type;

    Address = address.Trim();
    DisplayName = displayName?.CleanTrim();

    UserId = userId;

    new ReadOnlyRecipientValidator().ValidateAndThrow(this);
  }

  public RecipientType Type { get; }

  public string Address { get; }
  public string? DisplayName { get; }

  public AggregateId? UserId { get; }

  [JsonIgnore]
  public UserAggregate? User { get; set; }

  public static ReadOnlyRecipient From(UserAggregate user, RecipientType type = RecipientType.To)
  {
    if (user.Email == null)
    {
      throw new ArgumentException($"The {nameof(user.Email)} is required.", nameof(user));
    }

    return new ReadOnlyRecipient(user.Email.Address, user.FullName, type, user.Id)
    {
      User = user
    };
  }
}
