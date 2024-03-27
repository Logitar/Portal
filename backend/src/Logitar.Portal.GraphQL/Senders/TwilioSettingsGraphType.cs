using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.GraphQL.Senders;

internal class TwilioSettingsGraphType : ObjectGraphType<TwilioSettings>
{
  public TwilioSettingsGraphType()
  {
    Name = nameof(TwilioSettings);
    Description = "Represents the Twilio provider settings.";

    Field(x => x.AccountSid)
      .Description("The Twilio account Security ID used to contact the Twilio API.");
    Field(x => x.AuthenticationToken)
      .Description("The authentication token used to authorize Twilio API requests.");
  }
}
