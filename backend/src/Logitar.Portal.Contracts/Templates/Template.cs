using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Templates;

public class Template : Aggregate
{
  public string UniqueKey { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public Content Content { get; set; }

  public Realm? Realm { get; set; }

  public Template() : this(string.Empty, string.Empty, new Content())
  {
  }

  public Template(string uniqueKey, string subject, Content content)
  {
    UniqueKey = uniqueKey;
    Subject = subject;
    Content = content;
  }
}
