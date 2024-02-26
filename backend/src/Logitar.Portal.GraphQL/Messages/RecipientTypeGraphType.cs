using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.GraphQL.Messages;

internal class RecipientTypeGraphType : EnumerationGraphType<RecipientType>
{
  public RecipientTypeGraphType()
  {
    Name = nameof(RecipientType);
    Description = "Represents the available recipient types.";

    Add(RecipientType.Bcc, "The message will be sent as a blind carbon copy to the recipient, meaning other recipients will not see this recipient.");
    Add(RecipientType.CC, "The message will be sent as a carbon copy to the recipient.");
    Add(RecipientType.To, "The recipient is a primary recipient of the message.");
  }

  private void Add(RecipientType value, string description) => Add(value.ToString(), value, description);
}
