﻿using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions;

internal class SessionFacade : ISessionService
{
  private readonly IMediator _mediator;

  public SessionFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Session> RenewAsync(RenewSessionPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new RenewSessionCommand(payload), cancellationToken);
  }

  public async Task<Session> SignInAsync(SignInSessionPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SignInSessionCommand(payload), cancellationToken);
  }

  public async Task<Session?> SignOutAsync(string id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SignOutSessionCommand(id), cancellationToken);
  }
}
