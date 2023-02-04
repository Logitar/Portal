using Logitar.Portal.Domain.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands
{
  internal class DeleteApiKeyCommandHandler : IRequestHandler<DeleteApiKeyCommand>
  {
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteApiKeyCommandHandler(IRepository repository, IUserContext userContext)
    {
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteApiKeyCommand request, CancellationToken cancellationToken)
    {
      ApiKey apiKey = await _repository.LoadAsync<ApiKey>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<ApiKey>(request.Id);

      apiKey.Delete(_userContext.ActorId);

      await _repository.SaveAsync(apiKey, cancellationToken);

      return Unit.Value;
    }
  }
}
