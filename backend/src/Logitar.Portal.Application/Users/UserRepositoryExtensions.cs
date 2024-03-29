﻿using Logitar.Identity.Domain.Users;

namespace Logitar.Portal.Application.Users;

internal static class UserRepositoryExtensions
{
  public static async Task<UserAggregate?> LoadAsync(this IUserRepository repository, Guid id, CancellationToken cancellationToken = default)
  {
    UserId userId = new(id);
    return await repository.LoadAsync(userId, cancellationToken);
  }
}
