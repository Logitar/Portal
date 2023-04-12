using MediatR;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal class DeleteUsersHandler : IRequestHandler<DeleteUsers>
{
  private readonly ICurrentActor _currentActor;
  private readonly IUserRepository _userRepository;

  public DeleteUsersHandler(ICurrentActor currentActor, IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _userRepository = userRepository;
  }

  public async Task Handle(DeleteUsers request, CancellationToken cancellationToken)
  {
    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(request.Realm, cancellationToken);
    foreach (UserAggregate user in users)
    {
      user.Delete(_currentActor.Id);
    }

    await _userRepository.SaveAsync(users, cancellationToken);
  }
}
