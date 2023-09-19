using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL;
internal class ProviderSettingGraphType : ObjectGraphType<ProviderSetting>
{
  public ProviderSettingGraphType()
  {
    Name = nameof(ProviderSetting);
    Description = "Represents a setting of a sender provider.";

    Field(x => x.Key)
      .Description("The unique key of the provider setting.");
    Field(x => x.Value)
      .Description("The value of the provider setting.");
  }
}
