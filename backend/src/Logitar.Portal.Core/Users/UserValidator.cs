using FluentValidation;

namespace Logitar.Portal.Core.Users
{
  internal class UserValidator : AbstractValidator<User>
  {
    public UserValidator()
    {
      RuleFor(x => x.Username)
        .NotEmpty()
        .MaximumLength(256)
        .Must((user, username, context) =>
        {
          string? allowedUsernameCharacters = context.GetAllowedUsernameCharacters();

          return string.IsNullOrEmpty(allowedUsernameCharacters) || username.All(c => allowedUsernameCharacters.Contains(c));
        });

      When(x => x.PasswordHash == null, () => RuleFor(x => x.PasswordChangedAt).Null());

      RuleFor(x => x.Email)
        .MaximumLength(256)
        .EmailAddress();

      When(x => x.Email == null, () =>
      {
        RuleFor(x => x.EmailConfirmedAt).Null();
        RuleFor(x => x.EmailConfirmedById).Null();
      });

      When(x => x.IsEmailConfirmed, () =>
      {
        RuleFor(x => x.EmailConfirmedAt).NotNull();
        RuleFor(x => x.EmailConfirmedById).NotNull();
      });
      When(x => !x.IsEmailConfirmed, () =>
      {
        RuleFor(x => x.EmailConfirmedAt).Null();
        RuleFor(x => x.EmailConfirmedById).Null();
      });

      When(x => x.PhoneNumber == null, () =>
      {
        RuleFor(x => x.PhoneNumberConfirmedAt).Null();
        RuleFor(x => x.PhoneNumberConfirmedById).Null();
      });

      When(x => x.IsPhoneNumberConfirmed, () =>
      {
        RuleFor(x => x.PhoneNumberConfirmedAt).NotNull();
        RuleFor(x => x.PhoneNumberConfirmedById).NotNull();
      });
      When(x => !x.IsPhoneNumberConfirmed, () =>
      {
        RuleFor(x => x.PhoneNumberConfirmedAt).Null();
        RuleFor(x => x.PhoneNumberConfirmedById).Null();
      });

      RuleFor(x => x.FirstName)
        .MaximumLength(128);
      RuleFor(x => x.LastName)
        .MaximumLength(128);
      RuleFor(x => x.MiddleName)
        .MaximumLength(128);

      RuleFor(x => x.Locale)
        .Must(ValidationRules.BeAValidCulture);
      RuleFor(x => x.Picture)
        .MaximumLength(2048)
        .Must(ValidationRules.BeAValidUrl);

      When(x => x.IsDisabled, () =>
      {
        RuleFor(x => x.DisabledAt).NotNull();
        RuleFor(x => x.DisabledById).NotNull();
      });
      When(x => !x.IsDisabled, () =>
      {
        RuleFor(x => x.DisabledAt).Null();
        RuleFor(x => x.DisabledById).Null();
      });
    }
  }
}
