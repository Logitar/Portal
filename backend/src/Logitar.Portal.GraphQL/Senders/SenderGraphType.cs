using GraphQL.Types;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.GraphQL.Realms;

namespace Logitar.Portal.GraphQL.Senders;

internal class SenderGraphType : AggregateGraphType<Sender>
{
  public SenderGraphType() : base("Represents a sender in a mailing system. The sender is the actor who sends the message.")
  {
    Field(x => x.IsDefault)
      .Description("A value indicating whether or not the sender is the default in its realm.");

    //Field(x => x.EmailAddress)
    //  .Description("The email address of the sender."); // TODO(fpion): EmailAddress
    Field(x => x.DisplayName, nullable: true)
      .Description("The display name of the sender.");
    Field(x => x.Description, nullable: true)
      .Description("The description of the sender.");

    Field(x => x.Provider, type: typeof(NonNullGraphType<ProviderTypeGraphType>))
      .Description("The provider type of the sender.");
    // TODO(fpion): Mailgun
    // TODO(fpion): SendGrid

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm in which the sender resides.");
  }
}
