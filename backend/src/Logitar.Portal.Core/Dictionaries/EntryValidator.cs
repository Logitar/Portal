using FluentValidation;

namespace Logitar.Portal.Core.Dictionaries
{
  internal class EntryValidator : AbstractValidator<KeyValuePair<string, string>>
  {
    public EntryValidator()
    {
      RuleFor(x => x.Key)
        .NotEmpty()
        .MaximumLength(256)
        .Must(ValidationRules.BeAValidIdentifier);

      RuleFor(x => x.Value)
        .NotNull();
    }
  }
}
