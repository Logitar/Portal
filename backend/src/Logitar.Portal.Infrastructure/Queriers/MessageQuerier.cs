using Microsoft.EntityFrameworkCore;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Messages;

namespace Logitar.Portal.Infrastructure.Queriers
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

    public async Task<PagedList<Message>> GetPagedAsync(bool? hasErrors, bool? isDemo, string? realm, string? search, bool? succeeded, string? template,
      MessageSort? sort, bool desc,
      int? index, int? count,
      bool readOnly, CancellationToken cancellationToken)
    {
      IQueryable<Message> query = _messages.ApplyTracking(readOnly);

      if (realm == null)
      {
        query = query.Where(x => x.RealmId == null);
      }
      else
      {
        query = Guid.TryParse(realm, out Guid realmId)
          ? query.Where(x => x.RealmId == realmId)
          : query.Where(x => x.RealmAlias == realm.ToUpper());
      }

      if (hasErrors.HasValue)
      {
        query = query.Where(x => x.HasErrors == hasErrors.Value);
      }
      if (isDemo.HasValue)
      {
        query = query.Where(x => x.IsDemo == isDemo.Value);
      }
      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.SenderAddress, pattern)
              || EF.Functions.ILike(x.Subject, pattern)
              || EF.Functions.ILike(x.TemplateKey, pattern)
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
      if (template != null)
      {
        query = Guid.TryParse(template, out Guid templateId)
          ? query.Where(x => x.TemplateId == templateId)
          : query.Where(x => x.TemplateKey == template.ToUpper());
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          MessageSort.SentAt => desc ? query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt) : query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
          MessageSort.Subject => desc ? query.OrderByDescending(x => x.Subject) : query.OrderBy(x => x.Subject),
          _ => throw new ArgumentException($"The message sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      Message[] messages = await query.ToArrayAsync(cancellationToken);

      return new PagedList<Message>(messages, total);
    }
  }
}
