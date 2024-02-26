using GraphQL.Types;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.GraphQL;

internal class LocaleGraphType : ObjectGraphType<Locale>
{
  public LocaleGraphType()
  {
    Name = nameof(Locale);
    Description = "Represents a language in the system.";

    Field(x => x.Id)
      .Description("The unique identifier of the language.");
    Field(x => x.Code)
      .Description("The ISO 639-1 code of the language.");
    Field(x => x.DisplayName)
      .Description("The display name of the language.");
    Field(x => x.EnglishName)
      .Description("The name of the language in English.");
    Field(x => x.NativeName)
      .Description("The native name of the language.");
  }
}
