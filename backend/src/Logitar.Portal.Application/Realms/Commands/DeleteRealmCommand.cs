﻿using Logitar.EventSourcing;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal record DeleteRealmCommand(Guid Id) : Activity, IRequest<RealmModel?>;

internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, RealmModel?>
{
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public DeleteRealmCommandHandler(IRealmManager realmManager, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<RealmModel?> Handle(DeleteRealmCommand command, CancellationToken cancellationToken)
  {
    RealmId realmId = new(command.Id);
    Realm? realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    if (realm == null)
    {
      return null;
    }
    RealmModel result = await _realmQuerier.ReadAsync(realm, cancellationToken);

    ActorId actorId = command.ActorId;
    realm.Delete(actorId);
    await _realmManager.SaveAsync(realm, actorId, cancellationToken);

    return result;
  }
}
