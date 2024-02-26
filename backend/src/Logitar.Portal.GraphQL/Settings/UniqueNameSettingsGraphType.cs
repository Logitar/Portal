using GraphQL.Types;
using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.GraphQL.Settings;

internal class UniqueNameSettingsGraphType : ObjectGraphType<UniqueNameSettings>
{
  public UniqueNameSettingsGraphType()
  {
    Name = nameof(UniqueNameSettings);
    Description = "Represents the settings of user unique names.";

    Field(x => x.AllowedCharacters, nullable: true)
      .Description("A string containing all allowed characters in user unique names.");
  }
}
