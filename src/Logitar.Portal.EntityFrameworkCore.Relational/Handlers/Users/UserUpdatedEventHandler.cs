using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.Domain.Users.Events;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
{
  private readonly ICacheService _cacheService;
  private readonly PortalContext _context;
  private readonly IUserRepository _userRepository;

  public UserUpdatedEventHandler(ICacheService cacheService, PortalContext context,
    IUserRepository userRepository)
  {
    _cacheService = cacheService;
    _context = context;
    _userRepository = userRepository;
  }

  public async Task Handle(UserUpdatedEvent updated, CancellationToken cancellationToken)
  {
    UserAggregate? user = await _userRepository.LoadAsync(updated.AggregateId, updated.Version, includeDeleted: true, cancellationToken);
    if (user != null)
    {
      ActorEntity? actor = await _context.Actors
        .SingleOrDefaultAsync(x => x.Id == updated.AggregateId.Value, cancellationToken);
      if (actor == null)
      {
        actor = new(user);

        _context.Actors.Add(actor);
      }
      else
      {
        actor.Update(user);

        _cacheService.RemoveActor(new ActorId(actor.Id));
      }

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
