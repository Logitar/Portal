using FluentValidation;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Domain.Templates;

public record Content : IContent
{
  public string Type { get; }
  public string Text { get; }

  public Content(IContent content) : this(content.Type, content.Text)
  {
  }

  [JsonConstructor]
  private Content(string type, string text)
  {
    Type = type.Trim();
    Text = text.Trim();
    new ContentValidator().ValidateAndThrow(this);
  }

  public static Content Html(string text) => new(MediaTypeNames.Text.Html, text);
  public static Content PlainText(string text) => new(MediaTypeNames.Text.Plain, text);

  public Content Create(string text) => new(Type, text);
}
