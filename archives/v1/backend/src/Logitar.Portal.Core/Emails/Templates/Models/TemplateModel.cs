using Logitar.Portal.Core.Realms.Models;

namespace Logitar.Portal.Core.Emails.Templates.Models
{
  public class TemplateModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public string Key { get; set; } = null!;

    public string Subject { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string Contents { get; set; } = null!;

    public string? DisplayName { get; set; }
    public string? Description { get; set; }
  }
}
