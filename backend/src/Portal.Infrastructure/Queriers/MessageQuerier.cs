using Microsoft.EntityFrameworkCore;
using Portal.Core;
using Portal.Core.Emails.Messages;

namespace Portal.Infrastructure.Queriers
{
  internal class MessageQuerier : IMessageQuerier
  {
    private readonly DbSet<Message> _messages;

    public MessageQuerier(PortalDbContext dbContext)
    {
      _messages = dbContext.Messages;
    }

    public async Task<Message?> GetAsync(Guid id, bool readOnly, CancellationToken cancellationToken)
    {
      return await _messages.ApplyTracking(readOnly)
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PagedList<Message>> GetPagedAsync(bool? hasErrors, Guid? realmId, string? search, bool? succeeded, Guid? templateId,
      MessageSort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<Message> query = _messages.ApplyTracking(readOnly)
        .Where(x => realmId.HasValue ? x.RealmId == realmId.Value : x.RealmId == null);

      if (hasErrors.HasValue)
      {
        query = query.Where(x => x.HasErrors == hasErrors.Value);
      }
      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.SenderAddress, pattern)
              || EF.Functions.ILike(x.TemplateKey, pattern)
              || EF.Functions.ILike(x.TemplateSubject, pattern)
              || (x.RealmAlias != null && EF.Functions.ILike(x.RealmAlias, pattern))
              || (x.RealmName != null && EF.Functions.ILike(x.RealmName, pattern))
              || (x.SenderDisplayName != null && EF.Functions.ILike(x.SenderDisplayName, pattern))
              || (x.TemplateDisplayName != null && EF.Functions.ILike(x.TemplateDisplayName, pattern)));
          }
        }
      }
      if (succeeded.HasValue)
      {
        query = query.Where(x => x.Succeeded == succeeded.Value);
      }
      if (templateId.HasValue)
      {
        query = query.Where(x => x.TemplateId == templateId.Value);
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          MessageSort.SentAt => desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          MessageSort.Subject => desc ? query.OrderByDescending(x => x.TemplateSubject) : query.OrderBy(x => x.TemplateSubject),
          _ => throw new ArgumentException($"The message sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      Message[] messages = await query.ToArrayAsync(cancellationToken);

      return new PagedList<Message>(messages, total);
    }
  }
}
