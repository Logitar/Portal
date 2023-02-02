using FluentValidation;
using Logitar.Portal.Domain.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys
{
  internal class ApiKeyValidator : AbstractValidator<ApiKey>
  {
    public ApiKeyValidator()
    {
      RuleFor(x => x.SecretHash).NotEmpty();

      RuleFor(x => x.DisplayName).NotEmpty()
        .MaximumLength(256);

      RuleFor(x => x.Description).NullOrNotEmpty();
    }
  }
}
