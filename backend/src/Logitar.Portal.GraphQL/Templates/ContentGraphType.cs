using GraphQL.Types;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.GraphQL.Templates;

internal class ContentGraphType : ObjectGraphType<Content>
{
  public ContentGraphType()
  {
    Name = nameof(Content);
    Description = "Represents message contents.";

    Field(x => x.Type)
      .Description("The MIME type of the contents.");
    Field(x => x.Text)
      .Description("The text of the contents.");
  }
}
