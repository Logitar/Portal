using FluentValidation;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Validators;

internal class PhoneNumberValidator : AbstractValidator<string>
{
  public PhoneNumberValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Must(BeAValidPhoneNumber)
        .WithErrorCode(nameof(PhoneNumberValidator))
        .WithMessage("'{PropertyName}' must be a valid phone number.")
      .WithPropertyName(propertyName);
  }

  private static bool BeAValidPhoneNumber(string phoneNumber)
  {
    Phone phone = new(countryCode: null, phoneNumber, extension: null, phoneNumber);
    return phone.IsValid();
  }
}
