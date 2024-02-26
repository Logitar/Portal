using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.GraphQL.Messages;

internal class MessageStatusGraphType : EnumerationGraphType<MessageStatus>
{
  public MessageStatusGraphType()
  {
    Name = nameof(MessageStatus);
    Description = "Represents the available message statuses.";

    Add(MessageStatus.Failed, "An error occurred while sending the message.");
    Add(MessageStatus.Succeeded, "The message has been sent successfully.");
    Add(MessageStatus.Unsent, "The message has not been sent yet.");
  }

  private void Add(MessageStatus value, string description) => Add(value.ToString(), value, description);
}
