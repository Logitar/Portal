using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Tokens.Commands;
using MediatR;
using System.Text;
using System.Text.Json;

namespace Logitar.Portal.Core.Users.Commands;

internal class ResetPasswordHandler : IRequestHandler<ResetPassword, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IMediator _mediator;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ResetPasswordHandler(ICurrentActor currentActor,
    IMediator mediator,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _mediator = mediator;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(ResetPassword request, CancellationToken cancellationToken)
  {
    ResetPasswordInput input = request.Input;

    RealmAggregate realm = await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));

    ValidateTokenInput validateToken = new()
    {
      Token = input.Token,
      Purpose = Constants.PasswordRecovery.Purpose,
      Realm = realm.Id.ToGuid().ToString()
    };
    ValidatedToken validatedToken = await _mediator.Send(new ValidateToken(validateToken, Consume: true), cancellationToken);
    if (!validatedToken.Succeeded)
    {
      StringBuilder message = new();

      message.AppendLine("The password reset token validation failed.");

      message.AppendLine();
      message.AppendLine("Errors:");
      foreach (Error error in validatedToken.Errors)
      {
        message.Append(" - ").AppendLine(JsonSerializer.Serialize(error));
      }

      throw new InvalidCredentialsException(message.ToString());
    }

    Guid userId;
    if (validatedToken.Subject == null)
    {
      throw new InvalidOperationException($"The {nameof(validatedToken.Subject)} is required.");
    }
    else if (!Guid.TryParse(validatedToken.Subject, out userId))
    {
      throw new InvalidOperationException($"The subject '{validatedToken.Subject}' is not valid.");
    }

    UserAggregate user = await _userRepository.LoadAsync(userId, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(userId, nameof(input.Token));

    user.ChangePassword(_currentActor.Id, realm, input.Password);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
