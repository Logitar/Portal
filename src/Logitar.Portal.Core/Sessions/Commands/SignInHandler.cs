using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Contact;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal class SignInHandler : IRequestHandler<SignIn, Session>
{
  private readonly IRealmRepository _realmRepository;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignInHandler(IRealmRepository realmRepository,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _realmRepository = realmRepository;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<Session> Handle(SignIn request, CancellationToken cancellationToken)
  {
    RealmAggregate? realm = await LoadRealmAsync(request, cancellationToken);

    SignInInput input = request.Input;

    string username = input.Username.Trim();
    UserAggregate? user = await _userRepository.LoadAsync(realm, username, cancellationToken);
    if (user == null && realm?.RequireUniqueEmail == true)
    {
      ReadOnlyEmail email = new(input.Username);
      IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(realm, email, cancellationToken);
      if (users.Count() == 1)
      {
        user = users.Single();
      }
    }

    if (user == null)
    {
      throw new InvalidCredentialsException($"The user '{username}' could not be found in realm '{realm}'.");
    }

    SessionAggregate session = user.SignIn(realm, input.Password, input.Remember, input.IpAddress,
      input.AdditionalInformation, input.CustomAttributes?.ToDictionary());

    await _userRepository.SaveAsync(user, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session output = await _sessionQuerier.GetAsync(session, cancellationToken);
    output.RefreshToken = session.RefreshToken?.ToString();

    return output;
  }

  private async Task<RealmAggregate?> LoadRealmAsync(SignIn request, CancellationToken cancellationToken)
  {
    if (request.Realm == null)
    {
      return null;
    }

    return await _realmRepository.LoadAsync(request.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(request.Realm, nameof(request.Realm));
  }
}
