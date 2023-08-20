using AutoMapper;
using Logitar.Data;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class RealmQuerier : IRealmQuerier
{
  private readonly IMapper _mapper;
  private readonly IQueryHelper _queryHelper;
  private readonly DbSet<RealmEntity> _realms;

  public RealmQuerier(PortalContext context, IMapper mapper, IQueryHelper queryHelper)
  {
    _mapper = mapper;
    _realms = context.Realms;
    _queryHelper = queryHelper;
  }

  public async Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken)
    => await ReadAsync(realm.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity 'Id={realm.Id}' could not be found.");
  public async Task<Realm?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return _mapper.Map<Realm?>(realm); // TODO(fpion): Actors
  }

  public async Task<Realm?> ReadByUniqueSlugAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = uniqueSlug.Trim().ToUpper();

    RealmEntity? realm = await _realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);

    return _mapper.Map<Realm?>(realm); // TODO(fpion): Actors
  }

  public async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _queryHelper.From(PortalDb.Realms.Table).SelectAll(PortalDb.Realms.Table);
    _queryHelper.ApplyTextSearch(builder, payload.Id, PortalDb.Realms.AggregateId);
    _queryHelper.ApplyTextSearch(builder, payload.Search, PortalDb.Realms.UniqueSlug, PortalDb.Realms.DisplayName);

    long total = await _realms.FromQuery(builder.Build()).LongCountAsync(cancellationToken);

    // TODO(fpion): Sort

    // TODO(fpion): Skip
    // TODO(fpion): Limit

    return new SearchResults<Realm>(total); // TODO(fpion): Realms with Actors
  }
}
