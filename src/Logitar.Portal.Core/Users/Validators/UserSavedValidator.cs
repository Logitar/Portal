using FluentValidation;
using Logitar.Portal.Core.Users.Events;

namespace Logitar.Portal.Core.Users.Validators;

internal abstract class UserSavedValidator<T> : AbstractValidator<T> where T : UserSaved
{
  protected UserSavedValidator()
  {
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
