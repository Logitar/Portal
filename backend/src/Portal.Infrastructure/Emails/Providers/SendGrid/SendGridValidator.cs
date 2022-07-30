using FluentValidation;

namespace Portal.Infrastructure.Emails.Providers.SendGrid
{
  internal class SendGridValidator : AbstractValidator<SendGridSettings>
  {
    public SendGridValidator()
    {
      RuleFor(x => x.ApiKey)
        .NotEmpty();
    }
  }
}
