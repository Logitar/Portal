﻿using Logitar.Identity.Domain.Passwords;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.Passwords.Commands;

internal record DeleteOneTimePasswordCommand(Guid Id) : Activity, IRequest<OneTimePasswordModel?>;

internal class DeleteOneTimePasswordCommandHandler : IRequestHandler<DeleteOneTimePasswordCommand, OneTimePasswordModel?>
{
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;

  public DeleteOneTimePasswordCommandHandler(IOneTimePasswordQuerier oneTimePasswordQuerier, IOneTimePasswordRepository oneTimePasswordRepository)
  {
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
  }

  public async Task<OneTimePasswordModel?> Handle(DeleteOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    OneTimePasswordAggregate? oneTimePassword = await _oneTimePasswordRepository.LoadAsync(command.Id, cancellationToken);
    if (oneTimePassword == null || oneTimePassword.TenantId != command.TenantId)
    {
      return null;
    }
    OneTimePasswordModel result = await _oneTimePasswordQuerier.ReadAsync(command.Realm, oneTimePassword, cancellationToken);

    oneTimePassword.Delete(command.ActorId);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    return result;
  }
}