﻿using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts;
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

  public async Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
  }

  public async Task<User?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifier? identifier, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadUserQuery(id, uniqueName, identifier), cancellationToken);
  }

  public async Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SignOutUserCommand(id), cancellationToken);
  }
}
