using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class VerifyPhoneHandler : IRequestHandler<VerifyPhone, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public VerifyPhoneHandler(ICurrentActor currentActor,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(VerifyPhone request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    if (user.Phone != null)
    {
      user.SetPhone(_currentActor.Id, user.Phone.AsVerified());

      await _userRepository.SaveAsync(user, cancellationToken);
    }

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
