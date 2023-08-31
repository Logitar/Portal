using FluentValidation;

namespace Logitar.Portal.Domain.Users.Validators;

internal class PostalAddressValidator : AbstractValidator<PostalAddress>
{
  public PostalAddressValidator()
  {
    RuleFor(x => x.Street).NotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Locality).NotEmpty()
      .MaximumLength(byte.MaxValue);

    When(x => PostalAddressHelper.GetCountry(x.Country)?.Regions != null, () => RuleFor(x => x.Region).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Must((address, region) => PostalAddressHelper.GetCountry(address.Country)?.Regions?.Contains(region) == true)
      .WithErrorCode("RegionValidator")
      .WithMessage(address => $"'{{PropertyName}}' must be one of the following: {string.Join(", ", PostalAddressHelper.GetCountry(address.Country)?.Regions ?? new())}")
    ).Otherwise(() => When(x => x.Region != null, () => RuleFor(x => x.Region).NotEmpty()
      .MaximumLength(byte.MaxValue)
    ));

    When(x => PostalAddressHelper.GetCountry(x.Country)?.PostalCode != null, () => RuleFor(x => x.PostalCode).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Matches(address => PostalAddressHelper.GetCountry(address.Country)?.PostalCode ?? string.Empty)
      .WithErrorCode("PostalCodeValidator")
      .WithMessage(address => $"'{{PropertyName}}' must match the following expression: {PostalAddressHelper.GetCountry(address.Country)?.PostalCode}")
    ).Otherwise(() => When(x => x.PostalCode != null, () => RuleFor(x => x.PostalCode).NotEmpty()
      .MaximumLength(byte.MaxValue)
    ));

    RuleFor(x => x.Country).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Must(PostalAddressHelper.IsSupported)
      .WithErrorCode("CountryValidator")
      .WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", PostalAddressHelper.SupportedCountries)}");
  }
}
