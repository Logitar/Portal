using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class MessageQuerier : IMessageQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<MessageEntity> _messages;
  private readonly ISqlHelper _sqlHelper;

  public MessageQuerier(IActorService actorService, PortalContext context, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _messages = context.Messages;
    _sqlHelper = sqlHelper;
  }

  public async Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    MessageEntity? message = await _messages.AsNoTracking()
      .Include(x => x.Realm)
      .Include(x => x.Sender)
      .Include(x => x.Template)
      .Include(x => x.Recipients).ThenInclude(x => x.User)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);
    if (message == null)
    {
      return null;
    }

    return (await MapAsync(cancellationToken, message)).Single();
  }

  public async Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(Db.Messages.Table)
      .Join(Db.Realms.RealmId, Db.Messages.RealmId)
      .Join(Db.Templates.TemplateId, Db.Messages.TemplateId)
      .ApplyIdInFilter(Db.Messages.AggregateId, payload.IdIn)
      .SelectAll(Db.Messages.Table);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Messages.Subject);

    if (string.IsNullOrWhiteSpace(payload.Realm))
    {
      builder = builder.Where(Db.Messages.RealmId, Operators.IsNull());
    }
    else
    {
      string aggregateId = payload.Realm.Trim();
      string uniqueSlugNormalized = payload.Realm.Trim().ToUpper();
      builder = builder.WhereOr(
        new OperatorCondition(Db.Realms.AggregateId, Operators.IsEqualTo(aggregateId)),
        new OperatorCondition(Db.Realms.UniqueSlugNormalized, Operators.IsEqualTo(uniqueSlugNormalized))
      );
    }

    if (payload.IsDemo.HasValue)
    {
      builder = builder.Where(Db.Messages.IsDemo, Operators.IsEqualTo(payload.IsDemo.Value));
    }
    if (payload.Status.HasValue)
    {
      builder = builder.Where(Db.Messages.Status, Operators.IsEqualTo(payload.Status.Value));
    }
    if (!string.IsNullOrWhiteSpace(payload.Template))
    {
      string aggregateId = payload.Template.Trim();
      string uniqueNameNormalized = payload.Template.Trim().ToUpper();
      builder = builder.WhereOr(
        new OperatorCondition(Db.Templates.AggregateId, Operators.IsEqualTo(aggregateId)),
        new OperatorCondition(Db.Templates.UniqueNameNormalized, Operators.IsEqualTo(uniqueNameNormalized))
      );
    }

    IQueryable<MessageEntity> query = _messages.FromQuery(builder.Build())
      .Include(x => x.Realm)
      .Include(x => x.Sender)
      .Include(x => x.Template)
      .Include(x => x.Recipients).ThenInclude(x => x.User)
      .AsNoTracking();
    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<MessageEntity>? ordered = null;
    foreach (MessageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case MessageSort.RecipientCount:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.RecipientCount) : query.OrderBy(x => x.RecipientCount))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.RecipientCount) : ordered.ThenBy(x => x.RecipientCount));
          break;
        case MessageSort.Subject:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Subject) : query.OrderBy(x => x.Subject))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Subject) : ordered.ThenBy(x => x.Subject));
          break;
        case MessageSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    MessageEntity[] messages = await query.ToArrayAsync(cancellationToken);
    IEnumerable<Message> results = await MapAsync(cancellationToken, messages);

    return new SearchResults<Message>(results, total);
  }

  private async Task<IEnumerable<Message>> MapAsync(CancellationToken cancellationToken = default, params MessageEntity[] messages)
  {
    IEnumerable<ActorId> actorIds = messages.SelectMany(message => message.GetActorIds()).Distinct();
    Dictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return messages.Select(message => mapper.ToMessage(message));
  }
}
