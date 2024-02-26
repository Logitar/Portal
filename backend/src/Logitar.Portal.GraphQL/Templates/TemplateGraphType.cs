using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.GraphQL.Realms;

namespace Logitar.Portal.GraphQL.Templates;

internal class TemplateGraphType : AggregateGraphType<Template>
{
  public TemplateGraphType() : base("Represents a template in a mailing system. The template is the contents that will be rendered into a message.")
  {
    Field(x => x.UniqueKey)
      .Description("The unique key of the template.");
    Field(x => x.DisplayName, nullable: true)
      .Description("The display name of the template.");
    Field(x => x.Description, nullable: true)
      .Description("The description of the template.");

    Field(x => x.Subject)
      .Description("The subject of the messages sent using this template.");
    //Field(x => x.ContentType)
    //  .Description("The type of the contents of the template.");
    //Field(x => x.Contents)
    //  .Description("The contents of the template.");
    // TODO(fpion): Content

    Field(x => x.Realm, type: typeof(RealmGraphType))
      .Description("The realm in which the template resides.");
  }
}
