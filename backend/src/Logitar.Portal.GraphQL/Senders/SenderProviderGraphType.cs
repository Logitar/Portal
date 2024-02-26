using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal class SenderProviderGraphType : EnumerationGraphType<SenderProvider>
{
  public SenderProviderGraphType()
  {
    Name = nameof(SenderProvider);
    Description = "Represents the available sender providers.";

    Add(SenderProvider.Mailgun, "The sender sends messages through Mailgun.");
    Add(SenderProvider.SendGrid, "The sender sends messages through SendGrid.");
  }

  private void Add(SenderProvider value, string description) => Add(value.ToString(), value, description);
}
