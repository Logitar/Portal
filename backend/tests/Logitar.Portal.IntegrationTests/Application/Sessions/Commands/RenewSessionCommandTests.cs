﻿using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
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
    ApiKey apiKeyAggregate = new(new DisplayName("Default API key"), _passwordManager.GenerateBase64(256 / 8, out _));
    await _apiKeyRepository.SaveAsync(apiKeyAggregate);

    ApiKeyModel apiKey = new(apiKeyAggregate.DisplayName.Value)
    {
      Id = apiKeyAggregate.EntityId.ToGuid(),
      Version = apiKeyAggregate.Version,
      CreatedOn = apiKeyAggregate.CreatedOn.ToUniversalTime(),
      UpdatedOn = apiKeyAggregate.UpdatedOn.ToUniversalTime()
    };
    SetApiKey(apiKey);
    SetUser(user: null);

    SetRealm();

    User user = new(new UniqueName(Realm.UniqueNameSettings, UsernameString), actorId: null, UserId.NewId(TenantId));
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string currentSecret);
    ActorId actorId = new(apiKey.Id);
    Session session = user.SignIn(secret, actorId);
    await _userRepository.SaveAsync(user);
    await _sessionRepository.SaveAsync(session);

    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
      new("IpAddress", Faker.Internet.Ip())
    ];
    RenewSessionPayload payload = new(RefreshToken.Encode(session.Id, currentSecret), customAttributes);
    RenewSessionCommand command = new(payload);
    SessionModel result = await ActivityPipeline.ExecuteAsync(command);

    Assert.True(result.IsPersistent);
    Assert.NotNull(result.RefreshToken);
    Assert.True(result.IsActive);
    Assert.Null(result.SignedOutBy);
    Assert.Null(result.SignedOutOn);
    Assert.Equal(customAttributes, result.CustomAttributes);
    Assert.Equal(user.EntityId.ToGuid(), result.User.Id);
    Assert.Equal(session.EntityId.ToGuid(), result.Id);
    Assert.Equal(apiKey.Id, result.CreatedBy.Id);
    Assert.Equal(apiKey.Id, result.UpdatedBy.Id);
  }

  [Fact(DisplayName = "It should renew the session using the user ID when no actor.")]
  public async Task It_should_renew_the_session_using_the_user_Id_when_no_actor()
  {
    User user = (await _userRepository.LoadAsync(tenantId: null)).Single();
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string currentSecret);
    Session session = user.SignIn(secret);
    await _userRepository.SaveAsync(user);
    await _sessionRepository.SaveAsync(session);

    RenewSessionPayload payload = new(RefreshToken.Encode(session.Id, currentSecret));
    RenewSessionCommand command = new(payload);
    SessionModel result = await ActivityPipeline.ExecuteAsync(command);

    Guid actorId = new ActorId(user.Id.Value).ToGuid();
    Assert.Equal(session.EntityId.ToGuid(), result.Id);
    Assert.Equal(actorId, result.CreatedBy.Id);
    Assert.Equal(actorId, result.UpdatedBy.Id);
  }

  [Fact(DisplayName = "It should throw IncorrectSessionSecretException when the secret is not correct.")]
  public async Task It_should_throw_IncorrectSessionSecretException_when_the_secret_is_not_correct()
  {
    User user = (await _userRepository.LoadAsync(tenantId: null)).Single();
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out _);
    Session session = user.SignIn(secret);
    await _userRepository.SaveAsync(user);
    await _sessionRepository.SaveAsync(session);

    _ = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    RenewSessionPayload payload = new(RefreshToken.Encode(session.Id, secretString));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectSessionSecretException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(session.Id.Value, exception.SessionId);
    Assert.Equal(secretString, exception.AttemptedSecret);
  }

  [Fact(DisplayName = "It should throw InvalidRefreshTokenException when the refresh token is not valid.")]
  public async Task It_should_throw_InvalidRefreshTokenException_when_the_refresh_token_is_not_valid()
  {
    RenewSessionPayload payload = new(Guid.NewGuid().ToString());
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.RefreshToken, exception.RefreshToken);
    Assert.Equal("RefreshToken", exception.PropertyName);
    Assert.NotNull(exception.InnerException);
  }

  [Fact(DisplayName = "It should throw SessionIsNotActiveException when the session is not active.")]
  public async Task It_should_throw_SessionIsNotActiveException_when_the_session_is_not_active()
  {
    User user = (await _userRepository.LoadAsync(tenantId: null)).Single();
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    Session session = user.SignIn(secret);
    session.SignOut();
    await _userRepository.SaveAsync(user);
    await _sessionRepository.SaveAsync(session);

    RenewSessionPayload payload = new(RefreshToken.Encode(session.Id, secretString));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionIsNotActiveException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(session.Id.Value, exception.SessionId);
  }

  [Fact(DisplayName = "It should throw SessionIsNotPersistentException when the session is not persistent.")]
  public async Task It_should_throw_SessionIsNotPersistentException_when_the_session_is_not_persistent()
  {
    User user = (await _userRepository.LoadAsync(tenantId: null)).Single();
    Session session = user.SignIn();
    await _userRepository.SaveAsync(user);
    await _sessionRepository.SaveAsync(session);

    _ = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    RenewSessionPayload payload = new(RefreshToken.Encode(session.Id, secretString));
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionIsNotPersistentException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(session.Id.Value, exception.SessionId);
  }

  [Fact(DisplayName = "It should throw SessionNotFoundException when the session could not be found.")]
  public async Task It_should_throw_SessionNotFoundException_when_the_session_could_not_be_found()
  {
    RefreshToken refreshToken = new(SessionId.NewId(), RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _));
    RenewSessionPayload payload = new(refreshToken.Encode());
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(refreshToken.Id.TenantId?.ToGuid(), exception.TenantId);
    Assert.Equal(refreshToken.Id.EntityId.ToGuid(), exception.SessionId);
    Assert.Equal("RefreshToken", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw SessionNotFoundException when the session is in another realm.")]
  public async Task It_should_throw_SessionNotFoundException_when_the_session_is_in_another_realm()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());
    Session session = new(user);
    await _sessionRepository.SaveAsync(session);
    _ = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secret);

    SetRealm();

    string refreshToken = RefreshToken.Encode(session.Id, secret);
    RenewSessionPayload payload = new(refreshToken);
    RenewSessionCommand command = new(payload);

    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(TenantId.ToGuid(), exception.TenantId);
    Assert.Equal(session.EntityId.ToGuid(), exception.SessionId);
    Assert.Equal("RefreshToken", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    RenewSessionPayload payload = new();
    RenewSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("RefreshToken", exception.Errors.Single().PropertyName);
  }
}
