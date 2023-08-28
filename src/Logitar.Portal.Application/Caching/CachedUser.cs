using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Caching;

public record CachedUser(UserAggregate Aggregate, User Model);
