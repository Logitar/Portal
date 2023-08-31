using FluentValidation;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users.Validators;

namespace Logitar.Portal.Domain.Users;

public record PhoneNumber : IPhoneNumber
{
  public PhoneNumber(string number, string? countryCode = null, string? extension = null, bool isVerified = false)
  {
    CountryCode = countryCode?.CleanTrim();
    Number = number.Trim();
    Extension = extension?.CleanTrim();
    IsVerified = isVerified;

    new PhoneNumberValidator().ValidateAndThrow(this);
  }

  public string? CountryCode { get; }
  public string Number { get; } = string.Empty;
  public string? Extension { get; }
  public bool IsVerified { get; }

  public string FormatToE164() => PhoneNumberExtensions.FormatToE164(this);
}
