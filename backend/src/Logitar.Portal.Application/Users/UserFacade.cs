﻿using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users;

internal class UserFacade : IUserService
{
  private readonly IMediator _mediator;

  public UserFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateUserCommand(payload), cancellationToken);
  }

  public async Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SignOutUserCommand(id), cancellationToken);
  }
}
