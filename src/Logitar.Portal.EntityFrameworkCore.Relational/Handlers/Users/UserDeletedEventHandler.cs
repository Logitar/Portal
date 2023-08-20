using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.Domain.Users.Events;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
{
  private readonly ICacheService _cacheService;
  private readonly PortalContext _context;
  private readonly IUserRepository _userRepository;

  public UserDeletedEventHandler(ICacheService cacheService, PortalContext context,
    IUserRepository userRepository)
  {
    _cacheService = cacheService;
    _context = context;
    _userRepository = userRepository;
  }

  public async Task Handle(UserDeletedEvent @event, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors
      .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.Value, cancellationToken);
    if (actor == null)
    {
      UserAggregate? user = await _userRepository.LoadAsync(@event.AggregateId, @event.Version, includeDeleted: true, cancellationToken);
      if (user != null)
      {
        actor = new(user);

        _context.Actors.Add(actor);
      }
    }
    else
    {
      actor.Delete(@event);

      _cacheService.RemoveActor(new ActorId(actor.Id));
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
