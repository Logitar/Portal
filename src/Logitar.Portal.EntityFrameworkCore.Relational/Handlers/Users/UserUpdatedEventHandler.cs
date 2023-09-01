﻿using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
{
  private readonly PortalContext _context;

  public UserUpdatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(UserUpdatedEvent @event, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<UserEntity>(@event.AggregateId);

    HashSet<string> roleIds = @event.Roles.Keys.ToHashSet();
    RoleEntity[] roles = await _context.Roles
      .Where(x => roleIds.Contains(x.AggregateId))
      .ToArrayAsync(cancellationToken);

    user.Update(@event, roles);

    ActorEntity? actor = await _context.Actors
      .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
    if (actor == null)
    {
      actor = new(user);

      _context.Actors.Add(actor);
    }
    else if (@event.UniqueName != null || @event.Email != null || @event.FullName != null || @event.Picture != null)
    {
      actor.Update(user);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
