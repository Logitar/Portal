using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal class ChangePasswordHandler : IRequestHandler<ChangePassword, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ChangePasswordHandler(ICurrentActor currentActor,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(ChangePassword request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    RealmAggregate realm = await _realmRepository.LoadAsync(user, cancellationToken);

    ChangePasswordInput input = request.Input;

    user.ChangePassword(_currentActor.Id, realm, input.Password, input.Current);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
