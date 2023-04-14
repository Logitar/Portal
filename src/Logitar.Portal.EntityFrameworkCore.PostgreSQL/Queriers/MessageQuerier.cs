using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Messages;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Queriers;

internal class MessageQuerier : IMessageQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<MessageEntity> _messages;

  public MessageQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _messages = context.Messages;
  }

  public async Task<Message> GetAsync(MessageAggregate message, CancellationToken cancellationToken)
  {
    MessageEntity entity = await _messages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == message.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The message entity '{message.Id}' could not be found.");

    return _mapper.Map<Message>(entity);
  }

  public async Task<Message?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    MessageEntity? message = await _messages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Message>(message);
  }

  public async Task<PagedList<Message>> GetAsync(bool? hasErrors, bool? isDemo, string? realm, string? search, bool? succeeded, string? template,
    MessageSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    IQueryable<MessageEntity> query = _messages.AsNoTracking();

    if (hasErrors.HasValue)
    {
      query = query.Where(x => x.HasErrors == hasErrors.Value);
    }
    if (isDemo.HasValue)
    {
      query = query.Where(x => x.IsDemo == isDemo.Value);
    }

    if (realm == null)
    {
      query = query.Where(x => x.RealmId == null);
    }
    else if (Guid.TryParse(realm, out Guid realmId))
    {
      query = query.Where(x => x.RealmId == realmId);
    }
    else
    {
      query = query.Where(x => x.RealmUniqueNameNormalized == realm.ToUpper());
    }

    if (search != null)
    {
      foreach (string term in search.Split())
      {
        if (!string.IsNullOrEmpty(term))
        {
          string pattern = $"%{term}%";

          query = query.Where(x => EF.Functions.ILike(x.Subject, pattern)
            || EF.Functions.ILike(x.RealmUniqueName!, pattern)
            || EF.Functions.ILike(x.RealmDisplayName!, pattern)
            || EF.Functions.ILike(x.SenderEmailAddress, pattern)
            || EF.Functions.ILike(x.SenderDisplayName!, pattern)
            || EF.Functions.ILike(x.TemplateUniqueName, pattern)
            || EF.Functions.ILike(x.TemplateDisplayName!, pattern));
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
        : query.Where(x => x.TemplateUniqueName.ToUpper() == template.ToUpper());
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

    query = query.Page(skip, limit);

    MessageEntity[] messages = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Message>
    {
      Items = _mapper.Map<IEnumerable<Message>>(messages),
      Total = total
    };
  }
}
