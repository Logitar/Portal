using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Caching;

public record CachedUser(UserAggregate Aggregate, User Model);
