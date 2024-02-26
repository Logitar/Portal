using GraphQL.Types;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.GraphQL.Realms;

internal class RealmSortGraphType : EnumerationGraphType<RealmSort>
{
  public RealmSortGraphType()
  {
    Name = nameof(RealmSort);
    Description = "Represents the available realm fields for sorting.";

    Add(RealmSort.DisplayName, "The realms will be sorted by their display name.");
    Add(RealmSort.UniqueSlug, "The realms will be sorted by their unique slug.");
    Add(RealmSort.UpdatedOn, "The realms will be sorted by their latest update date and time.");
  }

  private void Add(RealmSort value, string description) => Add(value.ToString(), value, description);
}
