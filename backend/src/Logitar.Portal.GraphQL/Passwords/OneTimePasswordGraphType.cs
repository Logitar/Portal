using GraphQL.Types;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.GraphQL.Realms;

namespace Logitar.Portal.GraphQL.Passwords;

internal class OneTimePasswordGraphType : AggregateGraphType<OneTimePassword>
{
  public OneTimePasswordGraphType() : base("Represents a One-Time Password (OTP). These passwords are used to authenticate an user when their default password is unavailable, or in Multi-Factor Authentication (MFA) contexts.")
  {
    Field(x => x.Password, nullable: true)
      .Description("The generated One-Time Password (OTP).");

    Field(x => x.ExpiresOn, nullable: true)
      .Description("The expiration date and time of the One-Time Password (OTP).");
    Field(x => x.MaximumAttempts, nullable: true)
      .Description("The maximum number of attempts of the One-Time Password (OTP).");

    Field(x => x.AttemptCount)
      .Description("The current number of attempts of the One-Time Password (OTP).");
    Field(x => x.HasValidationSucceeded)
      .Description("A value indicating whether or not the One-Time Password (OTP) validation succeeded.");

    Field(x => x.CustomAttributes, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<CustomAttributeGraphType>>>))
      .Description("The custom attributes of the One-Time Password (OTP).");

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm in which the One-Time Password (OTP) resides.");
  }
}
