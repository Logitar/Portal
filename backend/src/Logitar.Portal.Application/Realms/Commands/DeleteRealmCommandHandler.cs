using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands
{
  internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand>
  {
    private readonly IRepository _repository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;

    public DeleteRealmCommandHandler(IRepository repository,
      ISessionRepository sessionRepository,
      IUserContext userContext,
      IUserRepository userRepository)
    {
      _repository = repository;
      _sessionRepository = sessionRepository;
      _userContext = userContext;
      _userRepository = userRepository;
    }

    public async Task<Unit> Handle(DeleteRealmCommand request, CancellationToken cancellationToken)
    {
      Realm realm = await _repository.LoadAsync<Realm>(request.Id, cancellationToken)
       ?? throw new EntityNotFoundException<Realm>(request.Id);

      await DeleteSessionsAsync(realm, cancellationToken);
      await DeleteUsersAsync(realm, cancellationToken);

      //await DeleteSendersAsync(realm, cancellationToken); // TODO(fpion): implement when Senders are completed
      //await DeleteTemplatesAsync(realm, cancellationToken); // TODO(fpion): implement when Templates are completed

      //await DeleteDictionariesAsync(realm, cancellationToken); // TODO(fpion): implement when Dictionaries are completed

      realm.Delete(_userContext.ActorId);

      await _repository.SaveAsync(realm, cancellationToken);

      return Unit.Value;
    }

    private async Task DeleteSessionsAsync(Realm realm, CancellationToken cancellationToken)
    {
      IEnumerable<Session> sessions = await _sessionRepository.LoadByRealmAsync(realm, cancellationToken);
      foreach (Session session in sessions)
      {
        session.Delete(_userContext.ActorId);
      }

      await _sessionRepository.SaveAsync(sessions, cancellationToken);
    }

    private async Task DeleteUsersAsync(Realm realm, CancellationToken cancellationToken)
    {
      IEnumerable<User> users = await _userRepository.LoadByRealmAsync(realm, cancellationToken);
      foreach (User user in users)
      {
        user.Delete(_userContext.ActorId);
      }

      await _userRepository.SaveAsync(users, cancellationToken);
    }
  }
}
