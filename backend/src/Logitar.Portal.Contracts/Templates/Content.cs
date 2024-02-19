namespace Logitar.Portal.Contracts.Templates;

public record Content : IContent
{
  public string Type { get; set; }
  public string Text { get; set; }

  public Content() : this(string.Empty, string.Empty)
  {
  }

  public Content(string type, string text)
  {
    Type = type;
    Text = text;
  }
}
