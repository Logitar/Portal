using Portal.Core.Realms.Models;

namespace Portal.Core.Emails.Templates.Models
{
  public class TemplateModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public string Key { get; set; } = null!;

    public string Contents { get; set; } = null!;

    public string? DisplayName { get; set; }
    public string? Description { get; set; }
  }
}
