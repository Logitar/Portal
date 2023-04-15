using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class ChangePasswordHandler : IRequestHandler<ChangePassword, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ChangePasswordHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(ChangePassword request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    RealmAggregate? realm = await _realmRepository.LoadAsync(user, cancellationToken);

    ChangePasswordInput input = request.Input;

    if (realm == null)
    {
      user.ChangePassword(_applicationContext.ActorId, _applicationContext.Configuration.PasswordSettings, input.Password, input.Current);
    }
    else
    {
      user.ChangePassword(_applicationContext.ActorId, realm, input.Password, input.Current);
    }

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
