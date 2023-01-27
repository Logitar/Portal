using FluentValidation;
using Logitar.Portal.Domain.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys
{
  internal class ApiKeyValidator : AbstractValidator<ApiKey>
  {
    public ApiKeyValidator()
    {
      RuleFor(x => x.KeyHash)
        .NotEmpty();

      RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(100);
    }
  }
}
