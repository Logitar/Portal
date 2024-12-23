using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class MessageQuerier : IMessageQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<MessageEntity> _messages;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<UserEntity> _users;

  public MessageQuerier(IActorService actorService, IdentityContext identityContext, PortalContext portalContext, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _messages = portalContext.Messages;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
    _users = identityContext.Users;
  }

  public async Task<Message> ReadAsync(RealmModel? realm, MessageAggregate message, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, message.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The message entity 'AggregateId={message.Id.Value}' could not be found.");
  }
  public async Task<Message?> ReadAsync(RealmModel? realm, MessageId id, CancellationToken cancellationToken)
    => await ReadAsync(realm, id.ToGuid(), cancellationToken);
  public async Task<Message?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    MessageEntity? message = await _messages.AsNoTracking()
      .Include(x => x.Recipients)
      .Include(x => x.Sender)
      .Include(x => x.Template)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    if (message == null || message.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(message, realm, cancellationToken);
  }

  public async Task<SearchResults<Message>> SearchAsync(RealmModel? realm, SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.QueryFrom(PortalDb.Messages.Table).SelectAll(PortalDb.Messages.Table)
      .LeftJoin(PortalDb.Templates.TemplateId, PortalDb.Messages.TemplateId)
      .ApplyRealmFilter(PortalDb.Messages.TenantId, realm)
      .ApplyIdFilter(PortalDb.Messages.AggregateId, payload.Ids);
    _searchHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Messages.Subject);

    if (payload.TemplateId.HasValue)
    {
      string aggregateId = new AggregateId(payload.TemplateId.Value).Value;
      builder.Where(PortalDb.Templates.AggregateId, Operators.IsEqualTo(aggregateId));
    }
    if (payload.IsDemo.HasValue)
    {
      builder.Where(PortalDb.Messages.IsDemo, Operators.IsEqualTo(payload.IsDemo.Value));
    }
    if (payload.Status.HasValue)
    {
      builder.Where(PortalDb.Messages.Status, Operators.IsEqualTo(payload.Status.Value.ToString()));
    }

    IQueryable<MessageEntity> query = _messages.FromQuery(builder).AsNoTracking()
      .Include(x => x.Recipients)
      .Include(x => x.Sender)
      .Include(x => x.Template);

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
    IEnumerable<Message> items = await MapAsync(messages, realm, cancellationToken);

    return new SearchResults<Message>(items, total);
  }

  private async Task<Message> MapAsync(MessageEntity message, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([message], realm, cancellationToken)).Single();
  private async Task<IEnumerable<Message>> MapAsync(IEnumerable<MessageEntity> messages, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = messages.SelectMany(message => message.GetActorIds());
    IEnumerable<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    HashSet<int> userIds = [];
    foreach (MessageEntity message in messages)
    {
      foreach (RecipientEntity recipient in message.Recipients)
      {
        if (recipient.UserId.HasValue)
        {
          userIds.Add(recipient.UserId.Value);
        }
      }
    }
    UserEntity[] users = await _users.AsNoTracking()
      .Where(u => userIds.Contains(u.UserId))
      .ToArrayAsync(cancellationToken);

    return messages.Select(message => mapper.ToMessage(message, realm, users));
  }
}
