using FluentValidation;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users.Events;

namespace Logitar.Portal.v2.Core.Users.Validators;

internal class UserCreatedValidator : AbstractValidator<UserCreated>
{
  public UserCreatedValidator(IUsernameSettings usernameSettings)
  {
    RuleFor(x => x.Username).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Username(usernameSettings);

    RuleFor(x => x.FirstName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.MiddleName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.LastName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.FullName).NullOrNotEmpty()
      .MaximumLength(ushort.MaxValue);

    RuleFor(x => x.Nickname).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Birthdate).Past();

    RuleFor(x => x.Locale).Locale();

    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
