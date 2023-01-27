using Logitar.Portal.Core2.Accounts.Payloads;
using Logitar.Portal.Core2.Realms;
using Logitar.Portal.Core2.Realms.Models;
using Logitar.Portal.Core2.Sessions;
using Logitar.Portal.Core2.Sessions.Models;
using Logitar.Portal.Core2.Users;
using MediatR;

namespace Logitar.Portal.Core2.Accounts.Commands
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

      SignInPayload payload = request.Payload;

      AggregateId? userId = (await _userQuerier.GetAsync(payload.Username, realm, cancellationToken))?.GetAggregateId();
      User? user = userId.HasValue ? await _repository.LoadAsync<User>(userId.Value, cancellationToken) : null;
      if (user == null || !_passwordService.IsMatch(user, payload.Password))
      {
        throw new InvalidCredentialsException();
      }
      user.EnsureIsTrusted(realm);

      return await _signInService.SignInAsync(user, payload.Remember, payload.IpAddress, payload.AdditionalInformation, cancellationToken);
    }
  }
}
