using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class CreateUserValidator : AbstractValidator<CreateUserPayload>
{
  public CreateUserValidator(IUserSettings userSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(userSettings.UniqueName));
  }
}
