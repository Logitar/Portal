using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders.Validators;

internal class UpdateSenderValidator : AbstractValidator<UpdateSenderPayload>
{
  public UpdateSenderValidator() : this(SenderProvider.SendGrid)
  {
  }
  public UpdateSenderValidator(SenderProvider provider)
  {
    SenderType type = provider.GetSenderType();
    switch (type)
    {
      case SenderType.Email:
        When(x => x.EmailAddress != null, () => RuleFor(x => x.EmailAddress!).EmailAddressInput());
        RuleFor(x => x.PhoneNumber).Empty();
        When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
        break;
      case SenderType.Sms:
        RuleFor(x => x.EmailAddress).Empty();
        When(x => x.PhoneNumber != null, () => RuleFor(x => x.EmailAddress!).PhoneNumber());
        RuleFor(x => x.DisplayName).Empty();
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }

    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));

    switch (provider)
    {
      case SenderProvider.Mailgun:
        RuleFor(x => x.SendGrid).Null();
        RuleFor(x => x.Twilio).Null();
        break;
      case SenderProvider.SendGrid:
        RuleFor(x => x.Mailgun).Null();
        RuleFor(x => x.Twilio).Null();
        break;
      case SenderProvider.Twilio:
        RuleFor(x => x.Mailgun).Null();
        RuleFor(x => x.SendGrid).Null();
        break;
      default:
        throw new SenderProviderNotSupportedException(provider);
    }
  }
}
