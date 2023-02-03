using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands
{
  internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand>
  {
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteRealmCommandHandler(IRepository repository, IUserContext userContext)
    {
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteRealmCommand request, CancellationToken cancellationToken)
    {
      Realm realm = await _repository.LoadAsync<Realm>(request.Id, cancellationToken)
       ?? throw new EntityNotFoundException<Realm>(request.Id);

      await DeleteSessionsAsync(realm, cancellationToken);
      await DeleteUsersAsync(realm, cancellationToken);

      await DeleteSendersAsync(realm, cancellationToken);
      await DeleteTemplatesAsync(realm, cancellationToken);

      //await DeleteDictionariesAsync(realm, cancellationToken); // TODO(fpion): implement when Dictionaries are completed

      realm.Delete(_userContext.ActorId);

      await _repository.SaveAsync(realm, cancellationToken);

      return Unit.Value;
    }

    private async Task DeleteSendersAsync(Realm realm, CancellationToken cancellationToken)
    {
      IEnumerable<Sender> senders = await _repository.LoadSendersByRealmAsync(realm, cancellationToken);
      foreach (Sender sender in senders)
      {
        sender.Delete(_userContext.ActorId);
      }

      await _repository.SaveAsync(senders, cancellationToken);
    }

    private async Task DeleteSessionsAsync(Realm realm, CancellationToken cancellationToken)
    {
      IEnumerable<Session> sessions = await _repository.LoadSessionsByRealmAsync(realm, cancellationToken);
      foreach (Session session in sessions)
      {
        session.Delete(_userContext.ActorId);
      }

      await _repository.SaveAsync(sessions, cancellationToken);
    }

    private async Task DeleteTemplatesAsync(Realm realm, CancellationToken cancellationToken)
    {
      IEnumerable<Template> templates = await _repository.LoadTemplatesByRealmAsync(realm, cancellationToken);
      foreach (Template template in templates)
      {
        template.Delete(_userContext.ActorId);
      }

      await _repository.SaveAsync(templates, cancellationToken);
    }

    private async Task DeleteUsersAsync(Realm realm, CancellationToken cancellationToken)
    {
      IEnumerable<User> users = await _repository.LoadUsersByRealmAsync(realm, cancellationToken);
      foreach (User user in users)
      {
        user.Delete(_userContext.ActorId);
      }

      await _repository.SaveAsync(users, cancellationToken);
    }
  }
}
