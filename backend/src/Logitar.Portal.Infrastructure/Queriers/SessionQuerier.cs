using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class SessionQuerier : ISessionQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<SessionEntity> _sessions;

    public SessionQuerier(IMappingService mapper, PortalContext context)
    {
      _mapper = mapper;
      _sessions = context.Sessions;
    }

    public async Task<SessionModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
      => await GetAsync(id.Value, cancellationToken);

    public async Task<SessionModel?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
      SessionEntity? session = await _sessions
        .Include(x => x.User).ThenInclude(x => x!.Realm)
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return await _mapper.MapAsync<SessionModel?>(session, cancellationToken);
    }
  }
}
