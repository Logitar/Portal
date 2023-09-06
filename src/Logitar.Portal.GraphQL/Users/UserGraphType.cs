using GraphQL.Types;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.GraphQL.Actors;
using Logitar.Portal.GraphQL.Realms;
using Logitar.Portal.GraphQL.Roles;

namespace Logitar.Portal.GraphQL.Users;

internal class UserGraphType : AggregateGraphType<User>
{
  public UserGraphType()
  {
    Name = nameof(User);
    Description = "Represents an user into the identity system. Users are actors who represent a person.";

    Field(x => x.Id)
      .Description("The unique identifier of the user.");

    Field(x => x.UniqueName)
      .Description("The unique name of the user.");

    Field(x => x.HasPassword)
      .Description("A value indicating whether or not the user has a password.");
    Field(x => x.PasswordChangedBy, type: typeof(ActorGraphType))
      .Description("The actor who changed the user's password lastly.");
    Field(x => x.PasswordChangedOn, nullable: true)
      .Description("The date and time when the user's password was changed lastly.");

    Field(x => x.DisabledBy, type: typeof(ActorGraphType))
      .Description("The actor who disabled the user.");
    Field(x => x.DisabledOn, nullable: true)
      .Description("The date and time when the user was disabled.");
    Field(x => x.IsDisabled)
      .Description("A value indicating whether or not the user is disabled.");

    Field(x => x.AuthenticatedOn, nullable: true)
      .Description("The date and time when the user was authenticated lastly.");

    Field(x => x.Address, type: typeof(AddressGraphType))
      .Description("The postal address of the user.");
    Field(x => x.Email, type: typeof(EmailGraphType))
      .Description("The email address of the user.");
    Field(x => x.Phone, type: typeof(PhoneGraphType))
      .Description("The phone number of the user.");

    Field(x => x.IsConfirmed)
     .Description("A value indicating whether or not the user is confirmed.");

    Field(x => x.FirstName, nullable: true)
      .Description("The first name of the user.");
    Field(x => x.MiddleName, nullable: true)
      .Description("The middle name of the user.");
    Field(x => x.LastName, nullable: true)
      .Description("The last name of the user.");
    Field(x => x.FullName, nullable: true)
      .Description("The full name of the user.");
    Field(x => x.Nickname, nullable: true)
      .Description("The nickname of the user.");

    Field(x => x.Birthdate, nullable: true)
      .Description("The date of birth of the user.");
    Field(x => x.Gender, nullable: true)
      .Description("The gender of the user.");
    Field(x => x.Locale, nullable: true)
      .Description("The code (ISO 639-1) of the user's locale (language).");
    Field(x => x.TimeZone, nullable: true)
      .Description("The identifier of the user's time zone, from the tz database.");

    Field(x => x.Picture, nullable: true)
      .Description("The URL to the user's picture.");
    Field(x => x.Profile, nullable: true)
      .Description("The URL to the user's profile page.");
    Field(x => x.Website, nullable: true)
      .Description("The URL to the user's website.");

    Field(x => x.CustomAttributes, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CustomAttributeGraphType>>>))
      .Description("The custom attributes of the user.");

    Field(x => x.Identifiers, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<IdentifierGraphType>>>))
      .Description("The roles of the user.");
    Field(x => x.Roles, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<RoleGraphType>>>))
      .Description("The roles of the user.");

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm in which the user resides.");
  }
}
