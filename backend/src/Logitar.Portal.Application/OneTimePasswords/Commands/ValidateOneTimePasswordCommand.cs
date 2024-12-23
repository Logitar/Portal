using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.OneTimePasswords.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

internal record ValidateOneTimePasswordCommand(Guid Id, ValidateOneTimePasswordPayload Payload) : Activity, IRequest<OneTimePasswordModel?>;

internal class ValidateOneTimePasswordCommandHandler : IRequestHandler<ValidateOneTimePasswordCommand, OneTimePasswordModel?>
{
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;

  public ValidateOneTimePasswordCommandHandler(IOneTimePasswordQuerier oneTimePasswordQuerier, IOneTimePasswordRepository oneTimePasswordRepository)
  {
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
  }

  public async Task<OneTimePasswordModel?> Handle(ValidateOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    ValidateOneTimePasswordPayload payload = command.Payload;
    new ValidateOneTimePasswordValidator().ValidateAndThrow(payload);

    OneTimePasswordId oneTimePasswordId = new(command.TenantId, new EntityId(command.Id));
    OneTimePassword? oneTimePassword = await _oneTimePasswordRepository.LoadAsync(oneTimePasswordId, cancellationToken);
    if (oneTimePassword == null || oneTimePassword.TenantId != command.TenantId)
    {
      return null;
    }

    ActorId actorId = command.ActorId;

    try
    {
      oneTimePassword.Validate(payload.Password, command.ActorId);
    }
    catch (IncorrectOneTimePasswordPasswordException)
    {
      await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);
      throw;
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      oneTimePassword.SetCustomAttribute(key, customAttribute.Value);
    }
    oneTimePassword.Update(actorId);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    return await _oneTimePasswordQuerier.ReadAsync(command.Realm, oneTimePassword, cancellationToken);
  }
}
