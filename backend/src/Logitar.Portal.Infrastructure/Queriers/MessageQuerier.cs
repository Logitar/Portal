using AutoMapper;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class MessageQuerier : IMessageQuerier
  {
    private readonly IMapper _mapper;
    private readonly DbSet<MessageEntity> _messages;

    public MessageQuerier(PortalContext context, IMapper mapper)
    {
      _mapper = mapper;
      _messages = context.Messages;
    }

    public async Task<MessageModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
    {
      MessageEntity? message = await _messages.AsNoTracking()
        .Include(x => x.Recipients)
        .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);

      return _mapper.Map<MessageModel>(message);
    }

    public async Task<ListModel<MessageModel>> GetPagedAsync(bool? hasErrors, bool? hasSucceeded, bool? isDemo, string? realm, string? search, string? template,
      MessageSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      IQueryable<MessageEntity> query = _messages.AsNoTracking();

      query = realm == null
        ? query.Where(x => x.RealmId == null)
        : query.Where(x => x.RealmAliasNormalized == realm.ToUpper() || x.RealmId == realm);

      if (hasErrors.HasValue)
      {
        query = query.Where(x => x.HasErrors == hasErrors.Value);
      }
      if (hasSucceeded.HasValue)
      {
        query = query.Where(x => x.HasSucceeded == hasSucceeded.Value);
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
              || (x.RealmDisplayName != null && EF.Functions.ILike(x.RealmDisplayName, pattern))
              || (x.SenderDisplayName != null && EF.Functions.ILike(x.SenderDisplayName, pattern))
              || (x.TemplateDisplayName != null && EF.Functions.ILike(x.TemplateDisplayName, pattern)));
          }
        }
      }
      if (template != null)
      {
        query.Where(x => x.TemplateId == template || x.TemplateKeyNormalized == template);
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          MessageSort.Subject => isDescending ? query.OrderByDescending(x => x.Subject) : query.OrderBy(x => x.Subject),
          MessageSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
          _ => throw new ArgumentException($"The message sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      MessageEntity[] messages = await query.ToArrayAsync(cancellationToken);

      return new ListModel<MessageModel>(_mapper.Map<IEnumerable<MessageModel>>(messages), total);
    }
  }
}
