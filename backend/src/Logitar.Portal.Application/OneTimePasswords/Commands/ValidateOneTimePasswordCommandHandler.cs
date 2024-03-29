﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Portal.Application.OneTimePasswords.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

internal class ValidateOneTimePasswordCommandHandler : IRequestHandler<ValidateOneTimePasswordCommand, OneTimePassword?>
{
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;

  public ValidateOneTimePasswordCommandHandler(IOneTimePasswordQuerier oneTimePasswordQuerier, IOneTimePasswordRepository oneTimePasswordRepository)
  {
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
  }

  public async Task<OneTimePassword?> Handle(ValidateOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    ValidateOneTimePasswordPayload payload = command.Payload;
    new ValidateOneTimePasswordValidator().ValidateAndThrow(payload);

    OneTimePasswordAggregate? oneTimePassword = await _oneTimePasswordRepository.LoadAsync(command.Id, cancellationToken);
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
      oneTimePassword.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    oneTimePassword.Update(actorId);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    return await _oneTimePasswordQuerier.ReadAsync(command.Realm, oneTimePassword, cancellationToken);
  }
}
