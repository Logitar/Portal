using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(PortalContext context, ILogger<UserCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        UserEntity user = new(notification);

        _context.Users.Add(user);
        _context.Actors.Add(new ActorEntity(user));

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
