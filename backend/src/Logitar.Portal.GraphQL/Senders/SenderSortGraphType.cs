using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal class SenderSortGraphType : EnumerationGraphType<SenderSort>
{
  public SenderSortGraphType()
  {
    Name = nameof(SenderSort);
    Description = "Represents the available sender fields for sorting.";

    Add(SenderSort.DisplayName, "The senders will be sorted by their display name.");
    Add(SenderSort.EmailAddress, "The senders will be sorted by their email address.");
    Add(SenderSort.PhoneNumber, "The senders will be sorted by their phone number.");
    Add(SenderSort.UpdatedOn, "The senders will be sorted by their latest update date and time.");
  }

  private void Add(SenderSort value, string description) => Add(value.ToString(), value, description);
}
