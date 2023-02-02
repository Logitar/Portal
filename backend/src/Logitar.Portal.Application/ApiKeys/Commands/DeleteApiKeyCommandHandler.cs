using FluentValidation;
using Logitar.Portal.Domain.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands
{
  internal class DeleteApiKeyCommandHandler : IRequestHandler<DeleteApiKeyCommand>
  {
    private readonly IValidator<ApiKey> _apiKeyValidator;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteApiKeyCommandHandler(IValidator<ApiKey> apiKeyValidator,
      IRepository repository,
      IUserContext userContext)
    {
      _apiKeyValidator = apiKeyValidator;
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteApiKeyCommand request, CancellationToken cancellationToken)
    {
      ApiKey apiKey = await _repository.LoadAsync<ApiKey>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<ApiKey>(request.Id);

      apiKey.Delete(_userContext.ActorId);
      _apiKeyValidator.ValidateAndThrow(apiKey);

      await _repository.SaveAsync(apiKey, cancellationToken);

      return Unit.Value;
    }
  }
}
