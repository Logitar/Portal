using FluentValidation;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys.Validators;

internal class AuthenticateApiKeyValidator : AbstractValidator<AuthenticateApiKeyPayload>
{
  public AuthenticateApiKeyValidator()
  {
    RuleFor(x => x.XApiKey).NotEmpty();
  }
}
