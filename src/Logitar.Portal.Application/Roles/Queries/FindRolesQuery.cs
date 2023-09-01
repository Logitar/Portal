using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record FindRolesQuery(IEnumerable<string> IdOrUniqueNames, string PropertyName, RealmAggregate? Realm) : IRequest<IEnumerable<RoleAggregate>>;
