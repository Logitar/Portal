using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.GraphQL.Settings;

namespace Logitar.Portal.GraphQL.Realms;

internal class RealmGraphType : AggregateGraphType<Realm>
{
  public RealmGraphType()
  {
    Name = nameof(Realm);
    Description = "Represents a hard tenant into the identity system.";

    Field(x => x.Id)
      .Description("The unique identifier of the realm.");

    Field(x => x.UniqueSlug)
      .Description("The unique slug of the realm.");
    Field(x => x.DisplayName, nullable: true)
      .Description("The display name of the realm.");
    Field(x => x.Description, nullable: true)
      .Description("The description of the realm.");

    Field(x => x.DefaultLocale, nullable: true)
      .Description("The code (ISO 639-1) of the default locale (language) of the realm.");
    Field(x => x.Secret)
      .Description("The secret to use to sign security tokens of the realm.");
    Field(x => x.Url, nullable: true)
      .Description("The primary URL of the application that is represented by the realm.");

    Field(x => x.RequireUniqueEmail)
      .Description("A value indicating whether or not email addresses are unique in the realm.");
    Field(x => x.RequireConfirmedAccount)
      .Description("A value indicating whether or not users in the realm need a confirmed account to sign-in.");

    Field(x => x.UniqueNameSettings, type: typeof(NonNullGraphType<UniqueNameSettingsGraphType>))
      .Description("The settings of the unique names in the realm.");
    Field(x => x.PasswordSettings, type: typeof(NonNullGraphType<PasswordSettingsGraphType>))
      .Description("The settings of the passwords in the realm.");

    Field(x => x.ClaimMappings, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<ClaimMappingGraphType>>>))
      .Description("The claim mappings of the realm.");

    Field(x => x.CustomAttributes, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CustomAttributeGraphType>>>))
      .Description("The custom attributes of the realm.");
  }
}
