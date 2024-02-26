using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.GraphQL.Users;

internal class PhoneGraphType : ContactGraphType<Phone>
{
  public PhoneGraphType() : base("Represents a phone number.")
  {
    Field(x => x.CountryCode, nullable: true)
      .Description("The code (ISO 3166-1 alpha-2) of the phone's country.");
    Field(x => x.Number)
      .Description("The phone number.");
    Field(x => x.Extension, nullable: true)
      .Description("The phone extension.");
    Field(x => x.E164Formatted)
      .Description("The phone number formatted as the E.164 international standard.");
  }
}
