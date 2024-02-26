using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record FindRolesQuery : IRequest<IEnumerable<FoundRole>>
{
  public TenantId? TenantId { get; }
  public IEnumerable<RoleModification> Roles { get; }
  public string? PropertyName { get; }

  public FindRolesQuery(TenantId? tenantId, IEnumerable<string> roles, string? propertyName = null)
    : this(tenantId, roles.Select(role => new RoleModification(role)), propertyName)
  {
  }

  public FindRolesQuery(TenantId? tenantId, IEnumerable<RoleModification> roles, string? propertyName = null)
  {
    TenantId = tenantId;
    Roles = roles;
    PropertyName = propertyName;
  }
}
