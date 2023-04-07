using Logitar.Portal.v2.Contracts.Users;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal class VerifyAddressHandler : IRequestHandler<VerifyAddress, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public VerifyAddressHandler(ICurrentActor currentActor,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(VerifyAddress request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    if (user.Address != null)
    {
      user.SetAddress(_currentActor.Id, user.Address.AsVerified());

      await _userRepository.SaveAsync(user, cancellationToken);
    }

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
