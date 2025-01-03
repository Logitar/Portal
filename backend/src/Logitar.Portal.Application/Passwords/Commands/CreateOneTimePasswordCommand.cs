﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Passwords.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.Passwords.Commands;

internal record CreateOneTimePasswordCommand(CreateOneTimePasswordPayload Payload) : Activity, IRequest<OneTimePasswordModel>;

internal class CreateOneTimePasswordCommandHandler : IRequestHandler<CreateOneTimePasswordCommand, OneTimePasswordModel>
{
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IPasswordManager _passwordManager;

  public CreateOneTimePasswordCommandHandler(IOneTimePasswordQuerier oneTimePasswordQuerier,
    IOneTimePasswordRepository oneTimePasswordRepository, IPasswordManager passwordManager)
  {
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
    _passwordManager = passwordManager;
  }

  public async Task<OneTimePasswordModel> Handle(CreateOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    CreateOneTimePasswordPayload payload = command.Payload;
    new CreateOneTimePasswordValidator().ValidateAndThrow(payload);

    OneTimePasswordId oneTimePasswordId = OneTimePasswordId.NewId(command.TenantId);
    OneTimePassword? oneTimePassword;
    if (payload.Id.HasValue)
    {
      oneTimePasswordId = new(command.TenantId, new EntityId(payload.Id.Value));
      oneTimePassword = await _oneTimePasswordRepository.LoadAsync(oneTimePasswordId, cancellationToken);
      if (oneTimePassword != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value, nameof(payload.Id));
      }
    }

    ActorId actorId = command.ActorId;

    Password password = _passwordManager.Generate(payload.Characters, payload.Length, out string passwordString);
    oneTimePassword = new(password, payload.ExpiresOn, payload.MaximumAttempts, actorId, oneTimePasswordId);

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      oneTimePassword.SetCustomAttribute(key, customAttribute.Value);
    }

    oneTimePassword.Update(actorId);
    await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    OneTimePasswordModel result = await _oneTimePasswordQuerier.ReadAsync(command.Realm, oneTimePassword, cancellationToken);
    result.Password = passwordString;
    return result;
  }
}
