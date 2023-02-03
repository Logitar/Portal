using Logitar.Portal.Application.Sessions;
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
    private readonly IRepository _repository;
    private readonly ISignInService _signInService;

    public RenewSessionCommandHandler(IPasswordService passwordService,
      IRepository repository,
      ISignInService signInService)
    {
      _passwordService = passwordService;
      _repository = repository;
      _signInService = signInService;
    }

    public async Task<SessionModel> Handle(RenewSessionCommand request, CancellationToken cancellationToken)
    {
      Realm? realm = null;
      if (request.Realm != null)
      {
        realm = await _repository.LoadRealmByAliasOrIdAsync(request.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(request.Realm);
      }

      RenewSessionPayload payload = request.Payload;

      if (!RenewToken.TryParse(payload.RenewToken, out RenewToken renewToken))
      {
        throw new InvalidCredentialsException();
      }

      Session? session = await _repository.LoadAsync<Session>(renewToken.Id, cancellationToken);
      User? user = session == null ? null : await _repository.LoadAsync<User>(session.UserId, cancellationToken);
      if (session?.KeyHash == null || !session.IsActive || !_passwordService.IsMatch(session, renewToken.Key)
        || user == null || user.RealmId != realm?.Id)
      {
        throw new InvalidCredentialsException();
      }
      user.EnsureIsTrusted(realm);

      return await _signInService.RenewAsync(session, payload.IpAddress, payload.AdditionalInformation, cancellationToken);
    }
  }
}
