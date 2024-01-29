using Logitar.EventSourcing;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Settings;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class RenewSessionCommandTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public RenewSessionCommandTests() : base()
  {
    _apiKeyRepository = ServiceProvider.GetRequiredService<IApiKeyRepository>();
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should renew the session using the provided actor ID.")]
  public async Task It_should_renew_the_session_using_the_provided_actor_Id()
  {
    ApiKeyAggregate apiKey = new(new DisplayNameUnit("Default API key"), _passwordManager.GenerateBase64(256 / 8, out _));
    await _apiKeyRepository.SaveAsync(apiKey);

    Actor actor = new(apiKey.DisplayName.Value)
    {
      Id = apiKey.Id.AggregateId.ToGuid(),
      Type = ActorType.ApiKey
    };
    SetActor(actor);

    Realm realm = new("tests", JwtSecretUnit.Generate().Value)
    {
      Id = Guid.NewGuid()
    };
    SetRealm(realm);

    TenantId tenantId = new(new AggregateId(realm.Id).Value);
    UserAggregate user = new(new UniqueNameUnit(realm.UniqueNameSettings, Faker.Person.UserName), tenantId);
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string currentSecret);
    ActorId actorId = new(actor.Id);
    SessionAggregate session = user.SignIn(secret, actorId);
    await _userRepository.SaveAsync(user);
    await _sessionRepository.SaveAsync(session);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
      new("IpAddress", Faker.Internet.Ip())
    ];
    RenewSessionPayload payload = new(RefreshToken.Encode(session.Id, currentSecret), customAttributes);
    RenewSessionCommand command = new(payload);
    Session result = await Mediator.Send(command);

    Assert.True(result.IsPersistent);
    Assert.NotNull(result.RefreshToken);
    Assert.True(result.IsActive);
    Assert.Null(result.SignedOutBy);
    Assert.Null(result.SignedOutOn);
    Assert.Equal(customAttributes, result.CustomAttributes);
    Assert.Equal(user.Id.AggregateId.ToGuid(), result.User.Id);
    Assert.Equal(session.Id.AggregateId.ToGuid(), result.Id);
    Assert.Equal(actorId.ToGuid(), result.CreatedBy.Id);
    Assert.Equal(actorId.ToGuid(), result.UpdatedBy.Id);
  }

  [Fact(DisplayName = "It should renew the session using the user ID when no actor.")]
  public async Task It_should_renew_the_session_using_the_user_Id_when_no_actor()
  {
    UserAggregate user = (await _userRepository.LoadAsync(tenantId: null)).Single();
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string currentSecret);
    SessionAggregate session = user.SignIn(secret);
    await _userRepository.SaveAsync(user);
    await _sessionRepository.SaveAsync(session);

    RenewSessionPayload payload = new(RefreshToken.Encode(session.Id, currentSecret));
    RenewSessionCommand command = new(payload);
    Session result = await Mediator.Send(command);

    Guid actorId = new ActorId(user.Id.Value).ToGuid();
    Assert.Equal(session.Id.AggregateId.ToGuid(), result.Id);
    Assert.Equal(actorId, result.CreatedBy.Id);
    Assert.Equal(actorId, result.UpdatedBy.Id);
  }

  [Fact(DisplayName = "It should throw InvalidRefreshTokenException when the refresh token is not valid.")]
  public async Task It_should_throw_InvalidRefreshTokenException_when_the_refresh_token_is_not_valid()
  {
    RenewSessionPayload payload = new(Guid.NewGuid().ToString());
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.RefreshToken, exception.RefreshToken);
    Assert.Equal("RefreshToken", exception.PropertyName);
    Assert.NotNull(exception.InnerException);
  }

  [Fact(DisplayName = "It should throw SessionNotFoundException when the session could not be found.")]
  public async Task It_should_throw_SessionNotFoundException_when_the_session_could_not_be_found()
  {
    RefreshToken refreshToken = new(SessionId.NewId(), RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _));
    RenewSessionPayload payload = new(refreshToken.Encode());
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(refreshToken.Id, exception.Id);
    Assert.Equal("RefreshToken", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    RenewSessionPayload payload = new();
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("RefreshToken", exception.Errors.Single().PropertyName);
  }
}
