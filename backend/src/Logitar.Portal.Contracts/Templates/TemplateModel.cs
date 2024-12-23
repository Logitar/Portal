using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Templates;

public class TemplateModel : AggregateModel
{
  public string UniqueKey { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public ContentModel Content { get; set; }

  public RealmModel? Realm { get; set; }

  public TemplateModel() : this(string.Empty, string.Empty, new ContentModel())
  {
  }

  public TemplateModel(string uniqueKey, string subject, ContentModel content)
  {
    UniqueKey = uniqueKey;
    Subject = subject;
    Content = content;
  }
}
