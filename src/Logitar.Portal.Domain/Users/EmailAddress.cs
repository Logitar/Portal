using FluentValidation;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users.Validators;

namespace Logitar.Portal.Domain.Users;

public record EmailAddress : IEmailAddress
{
  public EmailAddress(string address, bool isVerified = false)
  {
    Address = address.Trim();
    IsVerified = isVerified;

    new EmailAddressValidator().ValidateAndThrow(this);
  }

  public string Address { get; }
  public bool IsVerified { get; }
}
