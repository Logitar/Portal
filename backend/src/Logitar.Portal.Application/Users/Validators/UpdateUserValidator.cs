using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class UpdateUserValidator : AbstractValidator<UpdateUserPayload>
{
  public UpdateUserValidator(IUserSettings userSettings)
  {
    When(x => x.UniqueName != null, () => RuleFor(x => x.UniqueName!).SetValidator(new UniqueNameValidator(userSettings.UniqueName)));
  }
}
