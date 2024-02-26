using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.GraphQL.Messages;

internal class MessageSortGraphType : EnumerationGraphType<MessageSort>
{
  public MessageSortGraphType()
  {
    Name = nameof(MessageSort);
    Description = "Represents the available message fields for sorting.";

    Add(MessageSort.RecipientCount, "The messages will be sorted by their number of recipients.");
    Add(MessageSort.Subject, "The messages will be sorted by their subject.");
    Add(MessageSort.UpdatedOn, "The messages will be sorted by their latest update date and time.");
  }

  private void Add(MessageSort value, string description) => Add(value.ToString(), value, description);
}
