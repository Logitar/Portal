namespace Logitar.Portal.Contracts.Templates;

public class Template : Aggregate
{
  public string UniqueName { get; set; } // TODO(fpion): rename
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public Content Content { get; set; }

  public Template() : this(string.Empty, string.Empty, new Content())
  {
  }

  public Template(string uniqueName, string subject, Content content)
  {
    UniqueName = uniqueName;
    Subject = subject;
    Content = content;
  }
}
