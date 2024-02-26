using GraphQL.Types;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.GraphQL.Dictionaries;

internal class DictionaryEntryGraphType : ObjectGraphType<DictionaryEntry>
{
  public DictionaryEntryGraphType()
  {
    Name = nameof(DictionaryEntry);
    Description = "Represents a textual entry in a dictionary.";

    Field(x => x.Key)
      .Description("The unique key of the dictionary entry.");
    Field(x => x.Value)
      .Description("The value of the dictionary entry.");
  }
}
