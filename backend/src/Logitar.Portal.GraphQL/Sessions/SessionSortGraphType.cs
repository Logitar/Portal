using GraphQL.Types;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.GraphQL.Sessions;

internal class SessionSortGraphType : EnumerationGraphType<SessionSort>
{
  public SessionSortGraphType()
  {
    Name = nameof(SessionSort);
    Description = "Represents the available session fields for sorting.";

    Add(SessionSort.SignedOutOn, "The sessions will be their sign-out date and time.");
    Add(SessionSort.UpdatedOn, "The sessions will be sorted by their latest update date and time.");
  }

  private void Add(SessionSort value, string description) => Add(value.ToString(), value, description);
}
