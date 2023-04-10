using AutoMapper;
using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Core.Sessions;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _sessions = context.Sessions;
  }

  public async Task<Session> GetAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    SessionEntity entity = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.ExternalIdentifiers)
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == session.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity '{session.Id}' could not be found.");

    return _mapper.Map<Session>(entity);
  }
}
