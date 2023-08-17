﻿using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

internal class UserService : IUserService
{
  private readonly IRequestPipeline _pipeline;

  public UserService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateUserCommand(payload), cancellationToken);
  }

  public async Task<User?> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteUserCommand(id), cancellationToken);
  }

  public async Task<User?> ReadAsync(string? id, string? realm, string? uniqueName, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadUserQuery(id, realm, uniqueName), cancellationToken);
  }
}
