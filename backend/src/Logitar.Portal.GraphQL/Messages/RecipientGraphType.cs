using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.GraphQL.Users;

namespace Logitar.Portal.GraphQL.Messages;

internal class RecipientGraphType : ObjectGraphType<RecipientModel>
{
  public RecipientGraphType()
  {
    Name = nameof(RecipientModel);
    Description = "Represents a recipient of a message.";

    Field(x => x.Type, type: typeof(NonNullGraphType<RecipientTypeGraphType>))
      .Description("The type of the recipient.");

    Field(x => x.Address, nullable: true)
      .Description("The email address of the recipient.");
    Field(x => x.DisplayName, nullable: true)
      .Description("The display name of the recipient.");

    Field(x => x.PhoneNumber, nullable: true)
      .Description("The phone number of the recipient.");

    Field(x => x.User, type: typeof(UserGraphType))
      .Description("The user representing the recipient.");
  }
}
