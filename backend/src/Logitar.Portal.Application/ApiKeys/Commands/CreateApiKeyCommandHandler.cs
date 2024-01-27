using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.ApiKeys.Validators;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, ApiKey>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;

  public CreateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext, IMediator mediator, IPasswordManager passwordManager)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _mediator = mediator;
    _passwordManager = passwordManager;
  }

  public async Task<ApiKey> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
  {
    CreateApiKeyPayload payload = command.Payload;
    new CreateApiKeyValidator().ValidateAndThrow(payload);

    ApiKeyId? apiKeyId = ApiKeyId.TryCreate(payload.Id);
    if (apiKeyId != null && await _apiKeyRepository.LoadAsync(apiKeyId, includeDeleted: true, cancellationToken) != null)
    {
      throw new IdentifierAlreadyUsedException<ApiKeyAggregate>(payload.Id!, nameof(payload.Id));
    }

    DisplayNameUnit displayName = new(payload.DisplayName);
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out string secretString);
    TenantId? tenantId = TenantId.TryCreate(_applicationContext.Realm?.Id);
    ActorId actorId = _applicationContext.ActorId;
    ApiKeyAggregate apiKey = new(displayName, secret, tenantId, actorId, apiKeyId)
    {
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    if (payload.ExpiresOn.HasValue)
    {
      apiKey.SetExpiration(payload.ExpiresOn.Value);
    }
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      apiKey.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    apiKey.Update(actorId);

    IEnumerable<RoleAggregate> roles = await _mediator.Send(new FindRolesQuery(apiKey.TenantId, payload.Roles, nameof(payload.Roles)), cancellationToken);
    foreach (RoleAggregate role in roles)
    {
      apiKey.AddRole(role, actorId);
    }

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    ApiKey result = await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
    result.XApiKey = XApiKey.Encode(apiKey.Id, secretString);
    return result;
  }
}
