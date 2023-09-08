using GraphQL.Types;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.GraphQL.Realms;
using Logitar.Portal.GraphQL.Roles;

namespace Logitar.Portal.GraphQL.ApiKeys;

internal class ApiKeyGraphType : AggregateGraphType<ApiKey>
{
  public ApiKeyGraphType() : base("Represents an API key into the identity system. API keys are actors used to represent systems.")
  {
    Field(x => x.Id)
      .Description("The unique identifier of the API key.");

    Field(x => x.DisplayName)
      .Description("The display name of the API key.");
    Field(x => x.Description, nullable: true)
      .Description("The description of the API key.");
    Field(x => x.ExpiresOn, nullable: true)
      .Description("The expiration date and time of the API key.");

    Field(x => x.AuthenticatedOn, nullable: true)
      .Description("The date and time when the API key was authenticated lastly.");

    Field(x => x.CustomAttributes, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CustomAttributeGraphType>>>))
      .Description("The custom attributes of the API key.");

    Field(x => x.Roles, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<RoleGraphType>>>))
      .Description("The roles of the API key.");

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm in which the API key resides.");
  }
}
