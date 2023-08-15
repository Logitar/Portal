using Logitar.Identity.Domain.Users;
using Logitar.Identity.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
{
  private readonly PortalContext _context;
  private readonly IUserRepository _userRepository;

  public UserUpdatedEventHandler(PortalContext context, IUserRepository userRepository)
  {
    _context = context;
    _userRepository = userRepository;
  }

  public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors
      .SingleOrDefaultAsync(x => x.Id == notification.AggregateId.Value, cancellationToken);
    if (actor != null)
    {
      UserAggregate? user = await _userRepository.LoadAsync(notification.AggregateId, notification.Version, cancellationToken);
      if (user != null)
      {
        actor.Update(user);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }
}
