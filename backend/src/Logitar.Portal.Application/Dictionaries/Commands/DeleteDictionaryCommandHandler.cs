using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands
{
  internal class DeleteDictionaryCommandHandler : IRequestHandler<DeleteDictionaryCommand>
  {
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteDictionaryCommandHandler(IRepository repository, IUserContext userContext)
    {
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteDictionaryCommand request, CancellationToken cancellationToken)
    {
      Dictionary dictionary = await _repository.LoadAsync<Dictionary>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Dictionary>(request.Id);

      dictionary.Delete(_userContext.ActorId);

      await _repository.SaveAsync(dictionary, cancellationToken);

      return Unit.Value;
    }
  }
}
