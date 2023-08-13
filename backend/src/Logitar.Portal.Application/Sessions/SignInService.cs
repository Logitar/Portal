using Logitar.Portal.Application.Accounts;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Sessions
{
  internal class SignInService : ISignInService
  {
    private const int SessionKeyLength = 32;

    private readonly IMappingService _mappingService;
    private readonly IPasswordService _passwordService;
    private readonly IRepository<Session> _repository;
    private readonly IRepository<User> _userRepository;

    public SignInService(
      IMappingService mappingService,
      IPasswordService passwordService,
      IRepository<Session> repository,
      IRepository<User> userRepository
    )
    {
      _mappingService = mappingService;
      _passwordService = passwordService;
      _repository = repository;
      _userRepository = userRepository;
    }

    public async Task<SessionModel> RenewAsync(Session session, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(session);

      string keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out byte[] keyBytes);
      session.Update(keyHash, ipAddress, additionalInformation);
      await _repository.SaveAsync(session, cancellationToken);

      var model = await _mappingService.MapAsync<SessionModel>(session, cancellationToken);
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
      await _repository.SaveAsync(session, cancellationToken);

      user.SignIn(session.CreatedAt);
      await _userRepository.SaveAsync(user, cancellationToken);

      var model = await _mappingService.MapAsync<SessionModel>(session, cancellationToken);
      model.RenewToken = keyBytes == null ? null : new SecureToken(model.Id, keyBytes).ToString();

      return model;
    }
  }
}
