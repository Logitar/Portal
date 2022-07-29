namespace Portal.Core.Emails.Messages
{
  public interface IMessageQuerier
  {
    Task<Message?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<PagedList<Message>> GetPagedAsync(bool? hasErrors = null, Guid? realmId = null, string? search = null, bool? succeeded = null, Guid? templateId = null,
      MessageSort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
