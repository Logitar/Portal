using FluentValidation;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders
{
  internal class SenderValidator : AbstractValidator<Sender>
  {
    public SenderValidator()
    {
      RuleFor(x => x.EmailAddress).NotEmpty()
        .MaximumLength(255)
        .EmailAddress();

      RuleFor(x => x.DisplayName).NullOrNotEmpty()
        .MaximumLength(255);

      RuleFor(x => x.Provider).IsInEnum();

      RuleForEach(x => x.Settings.Keys).NotEmpty()
        .MaximumLength(255)
        .Identifier();
    }
  }
}
