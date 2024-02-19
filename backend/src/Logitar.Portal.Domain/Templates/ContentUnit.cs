using FluentValidation;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Domain.Templates;

public record ContentUnit : IContent
{
  public string Type { get; }
  public string Text { get; }

  public ContentUnit(IContent content) : this(content.Type, content.Text)
  {
  }

  private ContentUnit(string type, string text)
  {
    Type = type.Trim();
    Text = text.Trim();
    new ContentValidator().ValidateAndThrow(this);
  }

  public static ContentUnit Html(string text) => new(MediaTypeNames.Text.Html, text);
  public static ContentUnit PlainText(string text) => new(MediaTypeNames.Text.Plain, text);
}
