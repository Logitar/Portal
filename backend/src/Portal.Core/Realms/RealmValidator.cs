using FluentValidation;

namespace Portal.Core.Realms
{
  internal class RealmValidator : AbstractValidator<Realm>
  {
    public RealmValidator()
    {
      RuleFor(x => x.Alias)
        .NotEmpty()
        .MaximumLength(256)
        .Must(BeAlias);

      RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(256);
    }

    private static bool BeAlias(string? value) => value == null 
      || value.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
  }
}
