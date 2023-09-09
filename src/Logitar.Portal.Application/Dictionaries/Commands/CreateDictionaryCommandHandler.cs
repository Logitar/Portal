using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal class CreateDictionaryCommandHandler : IRequestHandler<CreateDictionaryCommand, Dictionary>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IRealmRepository _realmRepository;

  public CreateDictionaryCommandHandler(IApplicationContext applicationContext, IDictionaryQuerier dictionaryQuerier,
    IDictionaryRepository dictionaryRepository, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
    _realmRepository = realmRepository;
  }

  public async Task<Dictionary> Handle(CreateDictionaryCommand command, CancellationToken cancellationToken)
  {
    CreateDictionaryPayload payload = command.Payload;
    Locale locale = payload.Locale.GetRequiredLocale(nameof(payload.Locale));

    RealmAggregate? realm = null;
    if (payload.Realm != null)
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }
    string? tenantId = realm?.Id.Value;

    if (await _dictionaryRepository.LoadAsync(tenantId, locale, cancellationToken) != null)
    {
      string propertyName = string.Join(',', nameof(payload.Realm), nameof(payload.Locale));
      throw new DictionaryAlreadyExistingException(tenantId, locale, propertyName);
    }

    DictionaryAggregate dictionary = new(locale, tenantId, _applicationContext.ActorId);

    foreach (DictionaryEntry entry in payload.Entries)
    {
      dictionary.SetEntry(entry.Key, entry.Value);
    }

    dictionary.Update(_applicationContext.ActorId);

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(dictionary, cancellationToken);
  }
}
