using FluentValidation;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridValidator : AbstractValidator<SendGridSettings>
{
  public SendGridValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty();
  }
}
