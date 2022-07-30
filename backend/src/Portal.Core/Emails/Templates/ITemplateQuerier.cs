using Portal.Core.Realms;

namespace Portal.Core.Emails.Templates
{
  public interface ITemplateQuerier
  {
    Task<Template?> GetAsync(string key, Realm? realm = null, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<Template?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<PagedList<Template>> GetPagedAsync(Guid? realmId = null, string? search = null,
      TemplateSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
