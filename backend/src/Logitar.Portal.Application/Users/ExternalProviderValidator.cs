using FluentValidation;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  internal class ExternalProviderValidator : AbstractValidator<ExternalProvider>
  {
    public ExternalProviderValidator()
    {
      RuleFor(x => x.Key).NotEmpty()
        .MaximumLength(255);

      RuleFor(x => x.Value).NotEmpty()
        .MaximumLength(255);

      RuleFor(x => x.DisplayName).NullOrNotEmpty()
        .MaximumLength(255);
    }
  }
}
