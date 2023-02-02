using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class RenewSessionCommandHandler : IRequestHandler<RenewSessionCommand, SessionModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRealmRepository _realmRepository;
    private readonly IRepository _repository;
    private readonly ISignInService _signInService;

    public RenewSessionCommandHandler(IPasswordService passwordService,
      IRealmRepository realmRepository,
      IRepository repository,
      ISignInService signInService)
    {
      _passwordService = passwordService;
      _realmRepository = realmRepository;
      _repository = repository;
      _signInService = signInService;
    }

    public async Task<SessionModel> Handle(RenewSessionCommand request, CancellationToken cancellationToken)
    {
      Realm? realm = null;
      if (request.Realm != null)
      {
        realm = await _realmRepository.LoadByAliasOrIdAsync(request.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(request.Realm);
      }

      RenewSessionPayload payload = request.Payload;

      if (!SecureToken.TryParse(payload.RenewToken, out SecureToken secureToken))
      {
        throw new InvalidCredentialsException();
      }

      Session? session = await _repository.LoadAsync<Session>(secureToken.Id, cancellationToken);
      User? user = session == null ? null : await _repository.LoadAsync<User>(session.UserId, cancellationToken);
      if (session?.KeyHash == null || !session.IsActive || !_passwordService.IsMatch(session, secureToken.Key)
        || user == null || user.RealmId != realm?.Id)
      {
        throw new InvalidCredentialsException();
      }
      user.EnsureIsTrusted(realm);

      return await _signInService.RenewAsync(session, payload.IpAddress, payload.AdditionalInformation, cancellationToken);
    }
  }
}
