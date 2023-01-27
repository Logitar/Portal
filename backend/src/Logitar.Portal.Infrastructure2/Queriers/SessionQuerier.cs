using Logitar.Portal.Core2;
using Logitar.Portal.Core2.Sessions;
using Logitar.Portal.Core2.Sessions.Models;
using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure2.Queriers
{
  internal class SessionQuerier : ISessionQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<SessionEntity> _sessions;

    public SessionQuerier(PortalContext context, IMappingService mapper)
    {
      _mapper = mapper;
      _sessions = context.Sessions;
    }

    public async Task<SessionModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
    {
      SessionEntity? session = await _sessions.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == id.ToString(), cancellationToken);

      return await _mapper.MapAsync<SessionModel>(session, cancellationToken);
    }
  }
}
