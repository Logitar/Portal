using FluentValidation;

namespace Logitar.Portal.v2.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridValidator : AbstractValidator<SendGridSettings>
{
  public SendGridValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty();
  }
}
