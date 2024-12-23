using GraphQL.Types;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.GraphQL.Realms;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal class DictionaryGraphType : AggregateGraphType<DictionaryModel>
{
  public DictionaryGraphType() : base("Represents a textual resource dictionary that will be used to localized email templates.")
  {
    Field(x => x.Locale, type: typeof(NonNullGraphType<LocaleGraphType>))
      .Description("The language of the dictionary.");

    Field(x => x.EntryCount)
      .Description("The number of entries in the dictionary.");
    Field(x => x.Entries, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<DictionaryEntryGraphType>>>))
      .Description("The entries in the dictionary.");

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm in which the dictionary resides.");
  }
}
