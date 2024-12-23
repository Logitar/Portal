using FluentValidation;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Domain.Messages;

public record Recipient
{
  public RecipientType Type { get; }

  public string? Address { get; }
  public string? DisplayName { get; }

  public string? PhoneNumber { get; }

  public UserId? UserId { get; }
  [JsonIgnore]
  public User? User { get; }

  [Obsolete("Do not use this constructor. It will be removed in the next major release.")]
  public Recipient(string address, string? displayName = null, RecipientType type = RecipientType.To, UserId? userId = null)
    : this(type, address, displayName, phoneNumber: null, userId)
  {
  }

  [JsonConstructor]
  public Recipient(RecipientType type = RecipientType.To, string? address = null, string? displayName = null, string? phoneNumber = null, UserId? userId = null)
  {
    Type = type;
    Address = address?.CleanTrim();
    DisplayName = displayName?.CleanTrim();
    PhoneNumber = phoneNumber?.CleanTrim();
    UserId = userId;
    new Validator().ValidateAndThrow(this);
  }

  public Recipient(User user, RecipientType type = RecipientType.To)
    : this(type, user.Email?.Address, user.FullName, user.Phone?.FormatToE164(), user.Id)
  {
    User = user;
  }

  public MailAddress ToMailAddress() // ISSUE #467: move to Logitar.Portal.Infrastructure.Messages.MessageExtensions and remove System usings
  {
    if (Address == null)
    {
      throw new InvalidOperationException($"A recipient requires an email address in order to be converted into a {nameof(MailAddress)}.");
    }
    return new(Address, DisplayName);
  }

  private class Validator : AbstractValidator<Recipient>
  {
    public Validator()
    {
      RuleFor(x => x.Type).IsInEnum();

      RuleFor(x => x).Must(x => x.Address != null || x.PhoneNumber != null)
        .WithErrorCode("RecipientValidator")
        .WithMessage(x => $"At least one of the following must be specified: {nameof(x.Address)}, {nameof(x.PhoneNumber)}.");

      When(x => x.Address != null, () => RuleFor(x => x.Address!).SetValidator(new EmailAddressValidator()));
      When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName).NotEmpty());

      When(x => x.PhoneNumber != null, () => RuleFor(x => x.PhoneNumber!).SetValidator(new PhoneNumberValidator()));

      When(x => x.User != null, () => RuleFor(x => x.UserId).NotNull());
    }
  }
}
