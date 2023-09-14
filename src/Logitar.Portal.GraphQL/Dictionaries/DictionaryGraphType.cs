using GraphQL.Types;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.GraphQL.Realms;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal class DictionaryGraphType : AggregateGraphType<Dictionary>
{
  public DictionaryGraphType() : base("Represents a textual resource dictionary that will be used to localized email templates.")
  {
    Field(x => x.Id)
      .Description("The unique identifier of the dictionary.");

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm in which the dictionary resides.");
    Field(x => x.Locale)
      .Description("The code (ISO 639-1) of the dictionary's locale (language).");

    Field(x => x.Entries, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<DictionaryEntryGraphType>>>))
      .Description("The entries in the dictionary.");
    Field(x => x.EntryCount)
      .Description("The number of entries in the dictionary.");
  }
}
