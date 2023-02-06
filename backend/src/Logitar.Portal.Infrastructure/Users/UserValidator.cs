using FluentValidation;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  internal class UserValidator : AbstractValidator<User>
  {
    public UserValidator(IValidator<ExternalProvider> externalProviderValidator, UsernameSettings usernameSettings)
    {
      RuleFor(x => x.Username).NotEmpty()
        .MaximumLength(255)
        .Username(usernameSettings);

      RuleFor(x => x.PasswordHash).NullOrNotEmpty();

      RuleFor(x => x.Email).NullOrNotEmpty()
        .MaximumLength(255)
        .EmailAddress();

      RuleFor(x => x.PhoneNumber).NullOrNotEmpty();

      RuleFor(x => x.FirstName).NullOrNotEmpty()
        .MaximumLength(127);

      RuleFor(x => x.MiddleName).NullOrNotEmpty()
        .MaximumLength(127);

      RuleFor(x => x.LastName).NullOrNotEmpty()
        .MaximumLength(127);

      RuleFor(x => x.Locale).Locale();

      RuleFor(x => x.Picture).NullOrNotEmpty()
        .MaximumLength(2048)
        .Url();

      RuleForEach(x => x.ExternalProviders).SetValidator(externalProviderValidator);
    }
  }
}
