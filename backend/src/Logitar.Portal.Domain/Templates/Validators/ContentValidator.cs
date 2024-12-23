using FluentValidation;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Domain.Templates.Validators;

public class ContentValidator : AbstractValidator<IContent>
{
  public ContentValidator()
  {
    RuleFor(x => x.Type).ContentType();
    RuleFor(x => x.Text).NotEmpty();
  }
}
