using GraphQL.Types;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.GraphQL.Realms;

namespace Logitar.Portal.GraphQL.Roles;

internal class RoleGraphType : AggregateGraphType<Role>
{
  public RoleGraphType()
  {
    Name = nameof(Role);
    Description = "Represents a role into the identity system. Roles are typically used to assign permissions to actors.";

    Field(x => x.Id)
      .Description("The unique identifier of the role.");

    Field(x => x.UniqueName)
      .Description("The unique name of the role.");
    Field(x => x.DisplayName, nullable: true)
      .Description("The display name of the role.");
    Field(x => x.Description, nullable: true)
      .Description("The description of the role.");

    Field(x => x.CustomAttributes, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CustomAttributeGraphType>>>))
      .Description("The custom attributes of the role.");

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm in which the role resides.");
  }
}
