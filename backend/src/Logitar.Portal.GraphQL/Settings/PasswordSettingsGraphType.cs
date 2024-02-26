using GraphQL.Types;
using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.GraphQL.Settings;

internal class PasswordSettingsGraphType : ObjectGraphType<PasswordSettings>
{
  public PasswordSettingsGraphType()
  {
    Name = nameof(PasswordSettings);
    Description = "Represents the settings of user passwords.";

    Field(x => x.RequiredLength)
      .Description("The minimum number of characters in user passwords.");
    Field(x => x.RequiredUniqueChars)
      .Description("The minimum number of unique (different) characters in user passwords.");
    Field(x => x.RequireNonAlphanumeric)
      .Description("A value indicating whether or not user passwords require a non-alphanumeric character.");
    Field(x => x.RequireLowercase)
      .Description("A value indicating whether or not user passwords require a lowercase letter.");
    Field(x => x.RequireUppercase)
      .Description("A value indicating whether or not user passwords require an uppercase letter.");
    Field(x => x.RequireDigit)
      .Description("A value indicating whether or not user passwords require a digit.");
    Field(x => x.HashingStrategy)
      .Description("The key of the password hashing strategy to use.");
  }
}
