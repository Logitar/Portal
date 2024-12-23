namespace Logitar.Portal.Contracts.Templates;

public record ContentModel : IContent
{
  public string Type { get; set; }
  public string Text { get; set; }

  public ContentModel() : this(string.Empty, string.Empty)
  {
  }

  public ContentModel(IContent content) : this(content.Type, content.Text)
  {
  }

  public ContentModel(string type, string text)
  {
    Type = type;
    Text = text;
  }

  public static ContentModel Html(string text) => new(MediaTypeNames.Text.Html, text);
  public static ContentModel PlainText(string text) => new(MediaTypeNames.Text.Plain, text);
}
