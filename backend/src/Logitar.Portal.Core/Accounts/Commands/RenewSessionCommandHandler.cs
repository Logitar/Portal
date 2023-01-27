using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Tokens;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class RenewSessionCommandHandler : IRequestHandler<RenewSessionCommand, SessionModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository _repository;
    private readonly ISignInService _signInService;

    public RenewSessionCommandHandler(IPasswordService passwordService,
      IRealmQuerier realmQuerier,
      IRepository repository,
      ISignInService signInService)
    {
      _passwordService = passwordService;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _signInService = signInService;
    }

    public async Task<SessionModel> Handle(RenewSessionCommand request, CancellationToken cancellationToken)
    {
      RealmModel? realm = null;
      if (request.Realm != null)
      {
        realm = await _realmQuerier.GetAsync(request.Realm, cancellationToken)
          ?? throw EntityNotFoundException.Typed<Realm>(request.Realm);
      }

      SecureToken secureToken;
      try
      {
        secureToken = SecureToken.Parse(request.Payload.RenewToken);
      }
      catch (Exception innerException)
      {
        throw new InvalidCredentialsException(innerException);
      }

      Session? session = await _repository.LoadAsync<Session>(new AggregateId(secureToken.Id), cancellationToken);
      if (session?.KeyHash == null || !_passwordService.IsMatch(session, secureToken.Key))
      {
        throw new InvalidCredentialsException();
      }
      User user = await _repository.LoadAsync<User>(session.UserId, cancellationToken)
        ?? throw new InvalidCredentialsException();
      user.EnsureIsTrusted(realm);

      return await _signInService.RenewAsync(session, request.Payload.IpAddress, request.Payload.AdditionalInformation, cancellationToken);
    }
  }
}
