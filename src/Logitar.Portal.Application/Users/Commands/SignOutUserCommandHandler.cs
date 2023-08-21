using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class SignOutUserCommandHandler : IRequestHandler<SignOutUserCommand, User?>
{
  private readonly IAggregateRepository _aggregateRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SignOutUserCommandHandler(IAggregateRepository aggregateRepository,
    IApplicationContext applicationContext, ISessionRepository sessionRepository,
    IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _aggregateRepository = aggregateRepository;
    _applicationContext = applicationContext;
    _sessionRepository = sessionRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(SignOutUserCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = command.Id.GetAggregateId(nameof(command.Id));
    UserAggregate? user = await _userRepository.LoadAsync(id, cancellationToken);
    if (user == null)
    {
      return null;
    }

    IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadAsync(user, isActive: true, cancellationToken);
    foreach (SessionAggregate session in sessions)
    {
      session.SignOut(_applicationContext.ActorId);
    }
    await _aggregateRepository.SaveAsync(sessions, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
