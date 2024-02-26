using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal class SendGridSettingsGraphType : ObjectGraphType<SendGridSettings>
{
  public SendGridSettingsGraphType()
  {
    Name = nameof(SendGridSettings);
    Description = "Represents the SendGrid provider settings.";

    Field(x => x.ApiKey)
      .Description("The API key used to authorize calls to the SendGrid API.");
  }
}
