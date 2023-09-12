using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal class ProviderTypeGraphType : EnumerationGraphType<ProviderType>
{
  public ProviderTypeGraphType()
  {
    Name = nameof(ProviderType);
    Description = "Represents the available sender providers.";

    Add(ProviderType.SendGrid, "The sender sends message through SendGrid.");
  }

  private void Add(ProviderType value, string description) => Add(value.ToString(), value, description);
}
