using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.GraphQL.Users;

internal class EmailGraphType : ContactGraphType<EmailModel>
{
  public EmailGraphType() : base("Represents an email address.")
  {
    Field(x => x.Address)
      .Description("The email address.");
  }
}
