using FluentValidation;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
{
  internal class SendGridSettingsValidator : AbstractValidator<SendGridSettings>
  {
    public SendGridSettingsValidator()
    {
      RuleFor(x => x.ApiKey).NotEmpty();
    }
  }
}
