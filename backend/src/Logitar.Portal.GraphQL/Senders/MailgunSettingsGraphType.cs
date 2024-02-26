using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal class MailgunSettingsGraphType : ObjectGraphType<MailgunSettings>
{
  public MailgunSettingsGraphType()
  {
    Name = nameof(MailgunSettings);
    Description = "Represents the Mailgun provider settings.";

    Field(x => x.ApiKey)
      .Description("The API key used to authorize calls to the Mailgun API.");
    Field(x => x.DomainName)
      .Description("The domain name from which messages are sent.");
  }
}
