using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.GraphQL.Senders;
using Logitar.Portal.GraphQL.Settings;
using Logitar.Portal.GraphQL.Templates;

namespace Logitar.Portal.GraphQL.Realms;

internal class RealmGraphType : AggregateGraphType<Realm>
{
  public RealmGraphType() : base("Represents a hard tenant into the identity system.")
  {
    Field(x => x.Id)
      .Description("The unique identifier of the realm.");

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

    Field(x => x.PasswordRecoverySender, type: typeof(SenderGraphType))
      .Description("The sender used for password recovery in the realm.");
    Field(x => x.PasswordRecoveryTemplate, type: typeof(TemplateGraphType))
      .Description("The template used for password recovery in the realm.");
  }
}
