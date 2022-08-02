using FluentValidation;

namespace Logitar.Portal.Core.ApiKeys
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
