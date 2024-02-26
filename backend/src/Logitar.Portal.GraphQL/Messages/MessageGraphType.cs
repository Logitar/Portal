using GraphQL.Types;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.GraphQL.Realms;
using Logitar.Portal.GraphQL.Senders;
using Logitar.Portal.GraphQL.Templates;

namespace Logitar.Portal.GraphQL.Messages;

internal class MessageGraphType : AggregateGraphType<Message>
{
  public MessageGraphType() : base("Represents a message in a mailing system. The message is what content has been sent to its recipients.")
  {
    Field(x => x.Subject)
      .Description("The subject of the message.");
    Field(x => x.Body, type: typeof(ContentGraphType))
      .Description("The body of the message.");

    Field(x => x.RecipientCount)
      .Description("The number of recipients of the message.");
    Field(x => x.Recipients, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<RecipientGraphType>>>))
      .Description("The recipients of the message.");

    Field(x => x.Sender, type: typeof(NonNullGraphType<SenderGraphType>))
      .Description("The sender of the message.");
    Field(x => x.Template, type: typeof(NonNullGraphType<TemplateGraphType>))
      .Description("The template that has been used to compile the message contents.");

    Field(x => x.IgnoreUserLocale)
      .Description("A value indicating whether or not the user locale has been ignored when translating the message contents.");
    Field(x => x.Locale, type: typeof(LocaleGraphType))
      .Description("The language in which the message has been translated.");

    Field(x => x.Variables, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<VariableGraphType>>>))
      .Description("The variables used to inject values in the compiled message contents.");

    Field(x => x.IsDemo)
      .Description("A value indicating whether or not the message is a demo message.");

    Field(x => x.Status, type: typeof(NonNullGraphType<MessageStatusGraphType>))
      .Description("The status of the message.");
    Field(x => x.ResultData, type: typeof(NonNullGraphType<ListGraphType<NonNullGraphType<ResultDataGraphType>>>))
      .Description("The result data describing the message sending.");

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm through which the message has been sent.");
  }
}
