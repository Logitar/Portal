using Logitar.EventSourcing;
using Logitar.Portal.Application.Roles.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal class ReplaceApiKeyCommandHandler : IRequestHandler<ReplaceApiKeyCommand, ApiKey?>
{
  private readonly IApiKeyQuerier _apiKeyQuerier;
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IRealmRepository _realmRepository;

  public ReplaceApiKeyCommandHandler(IApiKeyQuerier apiKeyQuerier, IApiKeyRepository apiKeyRepository,
    IApplicationContext applicationContext, IMediator mediator, IRealmRepository realmRepository)
  {
    _apiKeyQuerier = apiKeyQuerier;
    _apiKeyRepository = apiKeyRepository;
    _applicationContext = applicationContext;
    _mediator = mediator;
    _realmRepository = realmRepository;
  }

  public async Task<ApiKey?> Handle(ReplaceApiKeyCommand command, CancellationToken cancellationToken)
  {
    ApiKeyAggregate? apiKey = await _apiKeyRepository.LoadAsync(command.Id, cancellationToken);
    if (apiKey == null)
    {
      return null;
    }

    RealmAggregate? realm = null;
    if (apiKey.TenantId != null)
    {
      realm = await _realmRepository.LoadAsync(apiKey, cancellationToken);
    }

    ApiKeyAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _apiKeyRepository.LoadAsync(apiKey.Id, command.Version.Value, cancellationToken);
    }

    ReplaceApiKeyPayload payload = command.Payload;

    if (reference == null || payload.Title.Trim() != reference.Title)
    {
      apiKey.Title = payload.Title;
    }
    if (reference == null || payload.Description?.CleanTrim() != reference.Description)
    {
      apiKey.Description = payload.Description;
    }
    if (reference == null || payload.ExpiresOn != reference.ExpiresOn)
    {
      apiKey.ExpiresOn = payload.ExpiresOn;
    }

    HashSet<string> customAttributeKeys = payload.CustomAttributes.Select(x => x.Key.Trim()).ToHashSet();
    foreach (string key in apiKey.CustomAttributes.Keys)
    {
      if (!customAttributeKeys.Contains(key) && (reference == null || reference.CustomAttributes.ContainsKey(key)))
      {
        apiKey.RemoveCustomAttribute(key);
      }
    }
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      apiKey.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }

    await SetRolesAsync(apiKey, reference, payload, realm, cancellationToken);

    apiKey.Update(_applicationContext.ActorId);

    await _apiKeyRepository.SaveAsync(apiKey, cancellationToken);

    return await _apiKeyQuerier.ReadAsync(apiKey, cancellationToken);
  }

  private async Task SetRolesAsync(ApiKeyAggregate apiKey, ApiKeyAggregate? reference,
    ReplaceApiKeyPayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    IEnumerable<RoleAggregate> roles = await _mediator.Send(new FindRolesQuery(payload.Roles, nameof(payload.Roles), realm), cancellationToken);

    HashSet<AggregateId> roleIds = roles.Select(r => r.Id).ToHashSet();
    foreach (AggregateId roleId in apiKey.Roles)
    {
      if (!roleIds.Contains(roleId) && (reference == null || reference.Roles.Contains(roleId)))
      {
        apiKey.RemoveRole(roleId);
      }
    }

    foreach (RoleAggregate role in roles)
    {
      apiKey.AddRole(role);
    }
  }
}
