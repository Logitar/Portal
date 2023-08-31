using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Role>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IRoleQuerier _roleQuerier;
  private readonly IRoleRepository _roleRepository;

  public CreateRoleCommandHandler(IApplicationContext applicationContext, IRealmRepository realmRepository, IRoleQuerier roleQuerier, IRoleRepository roleRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _roleQuerier = roleQuerier;
    _roleRepository = roleRepository;
  }

  public async Task<Role> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
  {
    CreateRolePayload payload = command.Payload;

    RealmAggregate? realm = null;
    if (payload.Realm != null)
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }
    string? tenantId = realm?.Id.Value;
    IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;

    if (await _roleRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException<RoleAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
    }

    RoleAggregate role = new(uniqueNameSettings, payload.UniqueName, tenantId, _applicationContext.ActorId)
    {
      DisplayName = payload.DisplayName,
      Description = payload.Description
    };

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      role.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    role.Update(_applicationContext.ActorId);

    await _roleRepository.SaveAsync(role, cancellationToken);

    return await _roleQuerier.ReadAsync(role, cancellationToken);
  }
}
