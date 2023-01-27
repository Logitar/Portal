using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class SignInCommandHandler : IRequestHandler<SignInCommand, SessionModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository _repository;
    private readonly ISignInService _signInService;
    private readonly IUserQuerier _userQuerier;

    public SignInCommandHandler(IPasswordService passwordService,
      IRealmQuerier realmQuerier,
      IRepository repository,
      ISignInService signInService,
      IUserQuerier userQuerier)
    {
      _passwordService = passwordService;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _signInService = signInService;
      _userQuerier = userQuerier;
    }

    public async Task<SessionModel> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
      RealmModel? realm = null;
      if (request.Realm != null)
      {
        realm = await _realmQuerier.GetAsync(request.Realm, cancellationToken)
          ?? throw EntityNotFoundException.Typed<Realm>(request.Realm, nameof(request.Realm));
      }

      AggregateId? userId = (await _userQuerier.GetAsync(request.Payload.Username, realm?.Id, cancellationToken))?.GetAggregateId();
      User? user = userId.HasValue ? await _repository.LoadAsync<User>(userId.Value, cancellationToken) : null;
      if (user == null || !_passwordService.IsMatch(user, request.Payload.Password))
      {
        throw new InvalidCredentialsException();
      }
      user.EnsureIsTrusted(realm);

      return await _signInService.SignInAsync(user, request.Payload.Remember, request.Payload.IpAddress, request.Payload.AdditionalInformation, cancellationToken);
    }
  }
}
