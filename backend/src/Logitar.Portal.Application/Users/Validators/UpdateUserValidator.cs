using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class UpdateUserValidator : AbstractValidator<UpdateUserPayload>
{
  public UpdateUserValidator(IUserSettings userSettings)
  {
    When(x => x.UniqueName != null, () => RuleFor(x => x.UniqueName!).SetValidator(new UniqueNameValidator(userSettings.UniqueName)));
    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new ChangePasswordValidator(userSettings.Password)));

    When(x => x.Address?.Value != null, () => RuleFor(x => x.Address!.Value!).SetValidator(new AddressValidator()));
    When(x => x.Email?.Value != null, () => RuleFor(x => x.Email!.Value!).SetValidator(new EmailValidator()));
    When(x => x.Phone?.Value != null, () => RuleFor(x => x.Phone!.Value!).SetValidator(new PhoneValidator()));
  }
}
