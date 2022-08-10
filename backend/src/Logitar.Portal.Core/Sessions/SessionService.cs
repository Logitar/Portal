using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Sessions
{
  internal class SessionService : ISessionService
  {
    private const int SessionKeyLength = 32;

    private readonly IMappingService _mappingService;
    private readonly IPasswordService _passwordService;
    private readonly ISessionQuerier _querier;
    private readonly IRepository<Session> _repository;
    private readonly IUserContext _userContext;
    private readonly IRepository<User> _userRepository;

    public SessionService(
      IMappingService mappingService,
      IPasswordService passwordService,
      ISessionQuerier querier,
      IRepository<Session> repository,
      IUserContext userContext,
      IRepository<User> userRepository
    )
    {
      _mappingService = mappingService;
      _passwordService = passwordService;
      _querier = querier;
      _repository = repository;
      _userContext = userContext;
      _userRepository = userRepository;
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

      return await _mappingService.MapAsync<Session, SessionModel>(sessions, cancellationToken);
    }

    public async Task<SessionModel> RenewAsync(Session session, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(session);

      string keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out byte[] keyBytes);
      session.Update(keyHash, ipAddress, additionalInformation);
      await _repository.SaveAsync(session, cancellationToken);

      var model = await _mappingService.MapAsync<SessionModel>(session, cancellationToken);
      model.RenewToken = new SecureToken(model.Id, keyBytes).ToString();

      return model;
    }

    public async Task<SessionModel> SignInAsync(User user, bool remember, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(user);

      byte[]? keyBytes = null;
      string? keyHash = null;
      if (remember)
      {
        keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out keyBytes);
      }

      var session = new Session(user, keyHash, ipAddress, additionalInformation);
      await _repository.SaveAsync(session, cancellationToken);

      user.SignIn(session.CreatedAt);
      await _userRepository.SaveAsync(user, cancellationToken);

      var model = await _mappingService.MapAsync<SessionModel>(session, cancellationToken);
      model.RenewToken = keyBytes == null ? null : new SecureToken(model.Id, keyBytes).ToString();

      return model;
    }

    public async Task<IEnumerable<SessionModel>> SignOutAllAsync(Guid userId, CancellationToken cancellationToken)
    {
      PagedList<Session> sessions = await _querier.GetPagedAsync(userId: userId, readOnly: false, cancellationToken: cancellationToken);

      foreach (Session session in sessions)
      {
        session.SignOut(_userContext.Actor.Id);
      }

      await _repository.SaveAsync(sessions, cancellationToken);

      return (await _mappingService.MapAsync<Session, SessionModel>(sessions, cancellationToken)).Items;
    }

    public async Task<SessionModel> SignOutAsync(Guid id, CancellationToken cancellationToken)
    {
      Session session = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Session>(id);

      session.SignOut();

      await _repository.SaveAsync(session, cancellationToken);

      return await _mappingService.MapAsync<SessionModel>(session, cancellationToken);
    }
  }
}
