using Logitar.Portal.Application.ApiKeys.Commands;
using Logitar.Portal.Application.ApiKeys.Queries;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys;

internal class ApiKeyFacade : IApiKeyService
{
  private readonly IMediator _mediator;

  public ApiKeyFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<ApiKey> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new AuthenticateApiKeyCommand(payload), cancellationToken);
  }

  public async Task<ApiKey> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateApiKeyCommand(payload), cancellationToken);
  }

  public async Task<ApiKey?> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteApiKeyCommand(id), cancellationToken);
  }

  public async Task<ApiKey?> ReadAsync(string? id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadApiKeyQuery(id), cancellationToken);
  }

  public async Task<ApiKey?> ReplaceAsync(string id, ReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceApiKeyCommand(id, payload, version), cancellationToken);
  }

  public async Task<ApiKey?> UpdateAsync(string id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateApiKeyCommand(id, payload), cancellationToken);
  }
}
