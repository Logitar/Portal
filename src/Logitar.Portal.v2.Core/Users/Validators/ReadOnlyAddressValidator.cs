using FluentValidation;
using Logitar.Portal.v2.Core.Users.Contact;

namespace Logitar.Portal.v2.Core.Users.Validators;

internal class ReadOnlyAddressValidator : AbstractValidator<ReadOnlyAddress>
{
  public ReadOnlyAddressValidator()
  {
    RuleFor(x => x.Line1).NotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Line2).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Locality).NotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Country).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Country();

    RuleFor(x => x.Region).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    When(x => PostalAddressHelper.GetCountry(x.Country)?.PostalCode != null,
      () => RuleFor(x => x.PostalCode).NullOrNotEmpty()
        .MaximumLength(byte.MaxValue)
        .Matches(x => PostalAddressHelper.GetCountry(x.Country)!.PostalCode))
      .Otherwise(() => RuleFor(x => x.PostalCode).NullOrNotEmpty()
        .MaximumLength(byte.MaxValue));

    When(x => PostalAddressHelper.GetCountry(x.Country)?.Regions != null,
      () => RuleFor(x => x.Region).NullOrNotEmpty()
        .MaximumLength(byte.MaxValue)
        .Must((x, r) => r == null || PostalAddressHelper.GetCountry(x.Country)!.Regions!.Contains(r)))
      .Otherwise(() => RuleFor(x => x.Region).NullOrNotEmpty()
        .MaximumLength(byte.MaxValue));
  }
}
