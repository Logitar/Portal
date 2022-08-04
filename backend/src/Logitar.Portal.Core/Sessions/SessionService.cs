using AutoMapper;
using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Sessions
{
  internal class SessionService : ISessionService
  {
    private const int SessionKeyLength = 32;

    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly IRepository<Session> _sessionRepository;
    private readonly IRepository<User> _userRepository;

    public SessionService(
      IMapper mapper,
      IPasswordService passwordService,
      IRepository<Session> sessionRepository,
      IRepository<User> userRepository
    )
    {
      _mapper = mapper;
      _passwordService = passwordService;
      _sessionRepository = sessionRepository;
      _userRepository = userRepository;
    }

    public async Task<SessionModel> RenewAsync(Session session, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(session);

      string keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out byte[] keyBytes);
      session.Update(keyHash, ipAddress, additionalInformation);
      await _sessionRepository.SaveAsync(session, cancellationToken);

      var model = _mapper.Map<SessionModel>(session);
      model.RenewToken = new SecureToken(model.Id, keyBytes).ToString();

      return model;
    }

    public async Task<SessionModel> SignInAsync(User user, bool remember, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(user);

      byte[]? keyBytes = null;
      string? keyHash = null;
      if (remember)
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
  }
}
