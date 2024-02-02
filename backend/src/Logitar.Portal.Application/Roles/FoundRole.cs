using Logitar.Identity.Domain.Roles;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application.Roles;

internal record FoundRole(RoleAggregate Role, CollectionAction Action);
