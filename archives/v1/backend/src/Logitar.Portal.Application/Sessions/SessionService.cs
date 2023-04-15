using Logitar.Portal.Application.Users;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Sessions
{
  internal class SessionService : ISessionService
  {
    private readonly IMappingService _mappingService;
    private readonly ISessionQuerier _querier;
    private readonly IRepository<Session> _repository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;

    public SessionService(
      IMappingService mappingService,
      ISessionQuerier querier,
      IRepository<Session> repository,
      IUserContext userContext,
      IUserQuerier userQuerier
    )
    {
      _mappingService = mappingService;
      _querier = querier;
      _repository = repository;
      _userContext = userContext;
      _userQuerier = userQuerier;
    }

    public async Task<SessionModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      Session? session = await _querier.GetAsync(id, readOnly: true, cancellationToken);
      if (session == null)
      {
        return null;
      }

      return await _mappingService.MapAsync<SessionModel>(session, cancellationToken);
    }

    public async Task<ListModel<SessionModel>> GetAsync(bool? isActive, bool? isPersistent, string? realm, Guid? userId,
      SessionSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<Session> sessions = await _querier.GetPagedAsync(isActive, isPersistent, realm, userId,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return await _mappingService.MapAsync<SessionModel>(sessions, cancellationToken);
    }

    public async Task<IEnumerable<SessionModel>> SignOutAllAsync(Guid userId, CancellationToken cancellationToken)
    {
      User user = await _userQuerier.GetAsync(userId, readOnly: true, cancellationToken)
        ?? throw new EntityNotFoundException<User>(userId);

      PagedList<Session> sessions = await _querier.GetPagedAsync(isActive: true, userId: user.Id, readOnly: false, cancellationToken: cancellationToken);

      foreach (Session session in sessions)
      {
        session.SignOut(_userContext.Actor.Id);
      }

      await _repository.SaveAsync(sessions, cancellationToken);

      return (await _mappingService.MapAsync<SessionModel>(sessions, cancellationToken)).Items;
    }

    public async Task<SessionModel> SignOutAsync(Guid id, CancellationToken cancellationToken)
    {
      Session session = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Session>(id);

      if (!session.IsActive)
      {
        throw new SessionAlreadySignedOutException(session);
      }

      session.SignOut();

      await _repository.SaveAsync(session, cancellationToken);

      return await _mappingService.MapAsync<SessionModel>(session, cancellationToken);
    }
  }
}
