using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Core.Realms;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Dictionaries.Commands;

internal class CreateDictionaryHandler : IRequestHandler<CreateDictionary, Dictionary>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IRealmRepository _realmRepository;

  public CreateDictionaryHandler(IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier,
    IDictionaryRepository dictionaryRepository,
    IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
    _realmRepository = realmRepository;
  }

  public async Task<Dictionary> Handle(CreateDictionary request, CancellationToken cancellationToken)
  {
    CreateDictionaryInput input = request.Input;

    RealmAggregate? realm = await LoadRealmAsync(input, cancellationToken);

    CultureInfo locale = input.Locale.GetRequiredCultureInfo(nameof(input.Locale));
    if (await _dictionaryRepository.LoadAsync(realm, locale, cancellationToken) != null)
    {
      throw new LocaleAlreadyUsedException(locale, nameof(input.Locale));
    }

    DictionaryAggregate dictionary = new(_applicationContext.ActorId, realm, locale, input.Entries?.ToDictionary());

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return await _dictionaryQuerier.GetAsync(dictionary, cancellationToken);
  }

  private async Task<RealmAggregate?> LoadRealmAsync(CreateDictionaryInput input, CancellationToken cancellationToken)
  {
    if (input.Realm == null)
    {
      return null;
    }

    return await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));
  }
}
