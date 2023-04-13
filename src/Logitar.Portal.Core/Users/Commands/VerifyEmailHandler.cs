using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class VerifyEmailHandler : IRequestHandler<VerifyEmail, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public VerifyEmailHandler(ICurrentActor currentActor,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(VerifyEmail request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    if (user.Email != null)
    {
      user.SetEmail(_currentActor.Id, user.Email.AsVerified());

      await _userRepository.SaveAsync(user, cancellationToken);
    }

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
