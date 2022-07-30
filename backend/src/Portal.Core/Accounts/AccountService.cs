using AutoMapper;
using Portal.Core.Accounts.Payloads;
using Portal.Core.Realms;
using Portal.Core.Sessions;
using Portal.Core.Sessions.Models;
using Portal.Core.Users;
using Portal.Core.Users.Models;
using Portal.Core.Users.Payloads;

namespace Portal.Core.Accounts
{
  internal class AccountService : IAccountService
  {
    private const int SessionKeyLength = 32;

    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly IRealmQuerier _realmQuerier;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly IRepository<Session> _sessionRepository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IRepository<User> _userRepository;
    private readonly IUserService _userService;

    public AccountService(
      IMapper mapper,
      IPasswordService passwordService,
      IRealmQuerier realmQuerier,
      ISessionQuerier sessionQuerier,
      IRepository<Session> sessionRepository,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IRepository<User> userRepository,
      IUserService userService
    )
    {
      _mapper = mapper;
      _passwordService = passwordService;
      _realmQuerier = realmQuerier;
      _sessionQuerier = sessionQuerier;
      _sessionRepository = sessionRepository;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _userRepository = userRepository;
      _userService = userService;
    }

    public async Task<UserModel> ChangePasswordAsync(ChangePasswordPayload payload, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(payload);

      _passwordService.ValidateAndThrow(payload.Password);

      User user = await _userQuerier.GetAsync(_userContext.Id, readOnly: false, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={_userContext.Id}' could not be found.");

      if (!_passwordService.IsMatch(user, payload.Current))
      {
        throw new InvalidCredentialsException();
      }

      var passwordHash = _passwordService.Hash(payload.Password);
      user.ChangePassword(passwordHash);
      await _userRepository.SaveAsync(user, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }

    public async Task<UserModel> GetProfileAsync(CancellationToken cancellationToken = default)
    {
      return await _userService.GetAsync(_userContext.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={_userContext.Id}' could not be found.");
    }

    public async Task<SessionModel> RenewSessionAsync(RenewSessionPayload payload, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(payload);

      if (!SecureToken.TryParse(payload.RenewToken, out SecureToken? secureToken) || secureToken == null)
      {
        throw new InvalidCredentialsException();
      }

      Session? session = await _sessionQuerier.GetAsync(secureToken.Id, readOnly: false, cancellationToken);
      if (session?.KeyHash == null || !session.IsActive || !_passwordService.IsMatch(session.KeyHash, secureToken.Key))
      {
        throw new InvalidCredentialsException();
      }

      byte[]? keyBytes = null;
      string? keyHash = null;
      if (session.KeyHash != null)
      {
        keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out keyBytes);
      }

      session.Update(keyHash, ipAddress, additionalInformation);
      await _sessionRepository.SaveAsync(session, cancellationToken);

      var model = _mapper.Map<SessionModel>(session);
      model.RenewToken = keyBytes == null ? null : new SecureToken(model.Id, keyBytes).ToString();

      return model;
    }

    public async Task<UserModel> SaveProfileAsync(UpdateUserPayload payload, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(payload);

      return await _userService.UpdateAsync(_userContext.Id, payload, cancellationToken);
    }

    public async Task<SessionModel> SignInAsync(SignInPayload payload, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = (Guid.TryParse(payload.Realm, out Guid guid)
          ? await _realmQuerier.GetAsync(guid, readOnly: false, cancellationToken)
          : await _realmQuerier.GetAsync(alias: payload.Realm, readOnly: false, cancellationToken)
        ) ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      User? user = await _userQuerier.GetAsync(payload.Username, realm, readOnly: false, cancellationToken);
      if (user == null || !_passwordService.IsMatch(user, payload.Password))
      {
        throw new InvalidCredentialsException();
      }
      else if (realm?.RequireConfirmedAccount == true && !user.IsAccountConfirmed)
      {
        throw new AccountNotConfirmedException(user.Id);
      }

      byte[]? keyBytes = null;
      string? keyHash = null;
      if (payload.Remember)
      {
        keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out keyBytes);
      }

      var session = new Session(user, keyHash, ipAddress, additionalInformation);
      await _sessionRepository.SaveAsync(session, cancellationToken);

      user.SignIn(session.CreatedAt);
      await _userRepository.SaveAsync(user, cancellationToken);

      var model = _mapper.Map<SessionModel>(session);
      model.RenewToken = keyBytes == null ? null : new SecureToken(model.Id, keyBytes).ToString();

      return model;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
      Session session = await _sessionQuerier.GetAsync(_userContext.SessionId, readOnly: false, cancellationToken)
        ?? throw new InvalidOperationException($"The session 'Id={_userContext.SessionId}' could not be found.");

      session.SignOut();
      await _sessionRepository.SaveAsync(session, cancellationToken);
    }
  }
}
