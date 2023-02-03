using FluentValidation;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands
{
  internal class UpdateDictionaryCommandHandler : IRequestHandler<UpdateDictionaryCommand, DictionaryModel>
  {
    private readonly IValidator<Dictionary> _dictionaryValidator;
    private readonly IDictionaryQuerier _dictionaryQuerier;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public UpdateDictionaryCommandHandler(IValidator<Dictionary> dictionaryValidator,
      IDictionaryQuerier dictionaryQuerier,
      IRepository repository,
      IUserContext userContext)
    {
      _dictionaryValidator = dictionaryValidator;
      _dictionaryQuerier = dictionaryQuerier;
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<DictionaryModel> Handle(UpdateDictionaryCommand request, CancellationToken cancellationToken)
    {
      Dictionary dictionary = await _repository.LoadAsync<Dictionary>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Dictionary>(request.Id);

      Dictionary<string, string>? entries = request.Payload.Entries?.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last().Value);
      dictionary.Update(_userContext.ActorId);
      _dictionaryValidator.ValidateAndThrow(dictionary);

      await _repository.SaveAsync(dictionary, cancellationToken);

      return await _dictionaryQuerier.GetAsync(dictionary.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The dictionary 'Id={dictionary.Id}' could not be found.");
    }
  }
}
