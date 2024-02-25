using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.GraphQL.Settings;

namespace Logitar.Portal.GraphQL.Realms;

internal class RealmGraphType : AggregateGraphType<Realm>
{
  public RealmGraphType() : base("Represents a hard tenant into the identity system.")
  {
    Field(x => x.UniqueSlug)
      .Description("The unique slug of the realm.");
    Field(x => x.DisplayName, nullable: true)
      .Description("The display name of the realm.");
    Field(x => x.Description, nullable: true)
      .Description("The description of the realm.");

    Field(x => x.DefaultLocale, type: typeof(LocaleGraphType))
      .Description("The default language of the realm.");
    Field(x => x.Secret)
      .Description("The secret to use to sign security tokens of the realm.");
    Field(x => x.Url, nullable: true)
      .Description("The primary URL of the application that is represented by the realm.");

    Field(x => x.UniqueNameSettings, type: typeof(NonNullGraphType<UniqueNameSettingsGraphType>))
      .Description("The settings of the unique names in the realm.");
    Field(x => x.PasswordSettings, type: typeof(NonNullGraphType<PasswordSettingsGraphType>))
      .Description("The settings of the passwords in the realm.");
    Field(x => x.RequireUniqueEmail)
      .Description("A value indicating whether or not email addresses are unique in the realm.");

    Field(x => x.CustomAttributes, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CustomAttributeGraphType>>>))
      .Description("The custom attributes of the realm.");
  }
}
