﻿using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmSessionsCommand(Realm Realm, ActorId ActorId) : INotification;

internal class DeleteRealmSessionsCommandHandler : INotificationHandler<DeleteRealmSessionsCommand>
{
  private readonly ISessionRepository _sessionRepository;

  public DeleteRealmSessionsCommandHandler(ISessionRepository sessionRepository)
  {
    _sessionRepository = sessionRepository;
  }

  public async Task Handle(DeleteRealmSessionsCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadAsync(tenantId, cancellationToken);

    foreach (SessionAggregate session in sessions)
    {
      session.Delete(command.ActorId);
    }

    await _sessionRepository.SaveAsync(sessions, cancellationToken);
  }
}
