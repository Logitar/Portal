using FluentValidation;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages.Validators;

namespace Logitar.Portal.Domain.Messages;

public record RecipientUnit
{
  public RecipientType Type { get; }

  public string Address { get; }
  public string? DisplayName { get; }

  public string? PhoneNumber { get; }

  public UserId? UserId { get; }
  [JsonIgnore]
  public UserAggregate? User { get; }

  [JsonConstructor]
  public RecipientUnit(string address, string? displayName = null, RecipientType type = RecipientType.To, UserId? userId = null)
  {
    Type = type;
    Address = address.Trim();
    DisplayName = displayName?.CleanTrim();
    UserId = userId;
    new RecipientValidator().ValidateAndThrow(this);
  }

  public RecipientUnit(UserAggregate user, RecipientType type = RecipientType.To)
  {
    if (user.Email == null)
    {
      throw new ArgumentException($"The {nameof(user.Email)} is required.", nameof(user));
    }

    Type = type;
    Address = user.Email.Address;
    DisplayName = user.FullName;
    UserId = user.Id;
    User = user;
  }

  public MailAddress ToMailAddress() => new(Address, DisplayName);
}
