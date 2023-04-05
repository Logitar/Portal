using Logitar.Portal.Application.Actors;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Application.Emails.Senders;
using Logitar.Portal.Application.Emails.Templates;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Emails.Senders;
using Logitar.Portal.Domain.Emails.Templates;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Realms.Mutations
{
  internal class DeleteRealmMutationHandler
  {
    private readonly IMappingService _mappingService;

    private readonly IDictionaryQuerier _dictionaryQuerier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly ISenderQuerier _senderQuerier;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly ITemplateQuerier _templateQuerier;
    private readonly IUserQuerier _userQuerier;

    private readonly IRepository<Dictionary> _dictionaryRepository;
    private readonly IRepository<Realm> _realmRepository;
    private readonly IRepository<Sender> _senderRepository;
    private readonly IRepository<Session> _sessionRepository;
    private readonly IRepository<Template> _templateRepository;
    private readonly IRepository<User> _userRepository;

    private readonly IActorService _actorService;

    private readonly IUserContext _userContext;

    public DeleteRealmMutationHandler(
      IMappingService mappingService,
      IDictionaryQuerier dictionaryQuerier,
      IRealmQuerier realmQuerier,
      ISenderQuerier senderQuerier,
      ISessionQuerier sessionQuerier,
      ITemplateQuerier templateQuerier,
      IUserQuerier userQuerier,
      IRepository<Dictionary> dictionaryRepository,
      IRepository<Realm> realmRepository,
      IRepository<Sender> senderRepository,
      IRepository<Session> sessionRepository,
      IRepository<Template> templateRepository,
      IRepository<User> userRepository,
      IActorService actorService,
      IUserContext userContext
    )
    {
      _mappingService = mappingService;
      _dictionaryQuerier = dictionaryQuerier;
      _realmQuerier = realmQuerier;
      _senderQuerier = senderQuerier;
      _sessionQuerier = sessionQuerier;
      _templateQuerier = templateQuerier;
      _userQuerier = userQuerier;
      _dictionaryRepository = dictionaryRepository;
      _realmRepository = realmRepository;
      _senderRepository = senderRepository;
      _sessionRepository = sessionRepository;
      _templateRepository = templateRepository;
      _userRepository = userRepository;
      _actorService = actorService;
      _userContext = userContext;
    }

    public async Task<RealmModel> Handle(DeleteRealmMutation request, CancellationToken cancellationToken)
    {
      Realm realm = await _realmQuerier.GetAsync(request.Id, readOnly: false, cancellationToken)
       ?? throw new EntityNotFoundException<Realm>(request.Id);

      await DeleteSessionsAsync(realm, cancellationToken);
      await DeleteUsersAsync(realm, cancellationToken);

      await DeleteSendersAsync(realm, cancellationToken);
      await DeleteTemplatesAsync(realm, cancellationToken);

      await DeleteDictionariesAsync(realm, cancellationToken);

      realm.Delete(_userContext.Actor.Id);

      await _realmRepository.SaveAsync(realm, cancellationToken);

      return await _mappingService.MapAsync<RealmModel>(realm, cancellationToken);
    }

    private async Task DeleteDictionariesAsync(Realm realm, CancellationToken cancellationToken)
    {
      PagedList<Dictionary> dictionaries = await _dictionaryQuerier.GetPagedAsync(realm: realm.Id.ToString(), readOnly: false, cancellationToken: cancellationToken);
      foreach (Dictionary dictionary in dictionaries)
      {
        dictionary.Delete(_userContext.Actor.Id);
      }
      await _dictionaryRepository.SaveAsync(dictionaries, cancellationToken);
    }

    private async Task DeleteSendersAsync(Realm realm, CancellationToken cancellationToken)
    {
      PagedList<Sender> senders = await _senderQuerier.GetPagedAsync(realm: realm.Id.ToString(), readOnly: false, cancellationToken: cancellationToken);
      foreach (Sender sender in senders)
      {
        sender.Delete(_userContext.Actor.Id);
      }
      await _senderRepository.SaveAsync(senders, cancellationToken);
    }

    private async Task DeleteSessionsAsync(Realm realm, CancellationToken cancellationToken)
    {
      PagedList<Session> sessions = await _sessionQuerier.GetPagedAsync(realm: realm.Id.ToString(), readOnly: false, cancellationToken: cancellationToken);
      foreach (Session session in sessions)
      {
        session.Delete(_userContext.Actor.Id);
      }
      await _sessionRepository.SaveAsync(sessions, cancellationToken);
    }

    private async Task DeleteTemplatesAsync(Realm realm, CancellationToken cancellationToken)
    {
      PagedList<Template> templates = await _templateQuerier.GetPagedAsync(realm: realm.Id.ToString(), readOnly: false, cancellationToken: cancellationToken);
      foreach (Template template in templates)
      {
        template.Delete(_userContext.Actor.Id);
      }
      await _templateRepository.SaveAsync(templates, cancellationToken);
    }

    private async Task DeleteUsersAsync(Realm realm, CancellationToken cancellationToken)
    {
      PagedList<User> users = await _userQuerier.GetPagedAsync(realm: realm.Id.ToString(), readOnly: false, cancellationToken: cancellationToken);
      foreach (User user in users)
      {
        user.Delete(_userContext.Actor.Id);
      }
      await _userRepository.SaveAsync(users, cancellationToken);
      await _actorService.SaveAsync(users, cancellationToken);
    }
  }
}
