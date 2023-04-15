using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class DeleteUsersHandler : IRequestHandler<DeleteUsers>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserRepository _userRepository;

  public DeleteUsersHandler(IApplicationContext applicationContext, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userRepository = userRepository;
  }

  public async Task Handle(DeleteUsers request, CancellationToken cancellationToken)
  {
    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(request.Realm, cancellationToken);
    foreach (UserAggregate user in users)
    {
      user.Delete(_applicationContext.ActorId);
    }

    await _userRepository.SaveAsync(users, cancellationToken);
  }
}
