﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Portal.Application.OneTimePasswords.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

internal class CreateOneTimePasswordCommandHandler : IRequestHandler<CreateOneTimePasswordCommand, OneTimePassword>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IPasswordManager _passwordManager;

  public CreateOneTimePasswordCommandHandler(IApplicationContext applicationContext,
    IOneTimePasswordQuerier oneTimePasswordQuerier, IOneTimePasswordRepository oneTimePasswordRepository, IPasswordManager passwordManager)
  {
    _applicationContext = applicationContext;
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
    _passwordManager = passwordManager;
  }

  public async Task<OneTimePassword> Handle(CreateOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    CreateOneTimePasswordPayload payload = command.Payload;
    new CreateOneTimePasswordValidator().ValidateAndThrow(payload);

    ActorId actorId = _applicationContext.ActorId;

    Password password = _passwordManager.Generate(payload.Characters, payload.Length, out string passwordString);
    OneTimePasswordAggregate oneTimePassword = new(password, _applicationContext.TenantId, payload.ExpiresOn, payload.MaximumAttempts, actorId);

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      oneTimePassword.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    oneTimePassword.Update(actorId);
    await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    OneTimePassword result = await _oneTimePasswordQuerier.ReadAsync(oneTimePassword, cancellationToken);
    result.Password = passwordString;
    return result;
  }
}
