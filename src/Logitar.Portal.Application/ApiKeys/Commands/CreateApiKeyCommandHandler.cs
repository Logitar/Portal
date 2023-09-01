using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, ApiKey>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;

  public CreateApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext, IMediator mediator, IPasswordService passwordService, IRealmRepository realmRepository)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _mediator = mediator;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
  }

  public async Task<ApiKey> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
  {
    CreateApiKeyPayload payload = command.Payload;

    RealmAggregate? realm = null;
    if (payload.Realm != null)
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }
    string? tenantId = realm?.Id.Value;
    IPasswordSettings passwordSettings = realm?.PasswordSettings ?? _applicationContext.Configuration.PasswordSettings;

    Password secret = _passwordService.Generate(passwordSettings, ApiKeyAggregate.SecretLength, out byte[] secretBytes);
    ApiKeyAggregate apiKey = new(payload.Title, secret, tenantId, _applicationContext.ActorId)
    {
      Description = payload.Description,
      ExpiresOn = payload.ExpiresOn
    };

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      apiKey.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    await SetRolesAsync(apiKey, payload, realm, cancellationToken);

    apiKey.Update(_applicationContext.ActorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    ApiKey result = await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
    result.XApiKey = new XApiKey(apiKey, secretBytes).Encode();

    return result;
  }

  private async Task SetRolesAsync(ApiKeyAggregate apiKey, CreateApiKeyPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IEnumerable<RoleAggregate> roles = await _mediator.Send(new FindRolesQuery(payload.Roles, nameof(payload.Roles), realm), cancellationToken);
    foreach (RoleAggregate role in roles)
    {
      apiKey.AddRole(role);
    }
  }
}
