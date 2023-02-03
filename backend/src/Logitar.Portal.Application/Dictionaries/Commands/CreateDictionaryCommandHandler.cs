using FluentValidation;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands
{
  internal class CreateDictionaryCommandHandler : IRequestHandler<CreateDictionaryCommand, DictionaryModel>
  {
    private readonly IValidator<Dictionary> _dictionaryValidator;
    private readonly IDictionaryQuerier _dictionaryQuerier;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public CreateDictionaryCommandHandler(IValidator<Dictionary> dictionaryValidator,
      IDictionaryQuerier dictionaryQuerier,
      IRepository repository,
      IUserContext userContext)
    {
      _dictionaryValidator = dictionaryValidator;
      _dictionaryQuerier = dictionaryQuerier;
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<DictionaryModel> Handle(CreateDictionaryCommand request, CancellationToken cancellationToken)
    {
      CreateDictionaryPayload payload = request.Payload;

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _repository.LoadRealmByAliasOrIdAsync(payload.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      if ((await _dictionaryQuerier.GetPagedAsync(payload.Locale, realm?.Id.ToString(), cancellationToken: cancellationToken)).Total > 0)
      {
        throw new DictionaryAlreadyExistingException(realm, payload.Locale);
      }

      Dictionary<string, string>? entries = payload.Entries?.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last().Value);
      Dictionary dictionary = new(_userContext.ActorId, payload.Locale, realm, entries);
      _dictionaryValidator.ValidateAndThrow(dictionary);

      await _repository.SaveAsync(dictionary, cancellationToken);

      return await _dictionaryQuerier.GetAsync(dictionary.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The dictionary 'Id={dictionary.Id}' could not be found.");
    }
  }
}
