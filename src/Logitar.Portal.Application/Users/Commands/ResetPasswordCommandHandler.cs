using Logitar.Portal.Application.Tokens.Commands;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IPasswordService _passwordService;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ResetPasswordCommandHandler(IApplicationContext applicationContext, IMediator mediator, IPasswordService passwordService,
    IRealmRepository realmRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _mediator = mediator;
    _passwordService = passwordService;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
  {
    ResetPasswordPayload payload = command.Payload;

    RealmAggregate realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    Password password = _passwordService.Create(realm.PasswordSettings, payload.Password);

    ValidateTokenPayload validateToken = new(payload.Token)
    {
      Consume = true,
      Type = PasswordRecovery.TokenType
    };
    ValidatedToken validatedToken = await _mediator.Send(new ValidateTokenCommand(validateToken, realm), cancellationToken);

    if (validatedToken == null || !Guid.TryParse(validatedToken.Subject, out Guid id))
    {
      throw new InvalidCredentialsException($"The {nameof(validatedToken.Subject)} claim is required and must be a valid user GUID.");
    }
    UserAggregate user = await _userRepository.LoadAsync(id, cancellationToken)
      ?? throw new InvalidCredentialsException($"The user 'Id={id}' could not be found.");
    user.ResetPassword(password, _applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
