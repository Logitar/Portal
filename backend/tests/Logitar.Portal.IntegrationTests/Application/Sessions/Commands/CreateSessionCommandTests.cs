﻿using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateSessionCommandTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public CreateSessionCommandTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should create a persistent session.")]
  public async Task It_should_create_a_persistent_session()
  {
    CreateSessionPayload payload = new(UsernameString)
    {
      IsPersistent = true
    };
    CreateSessionCommand command = new(payload);
    SessionModel session = await ActivityPipeline.ExecuteAsync(command);

    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Empty(session.CustomAttributes);

    User user = Assert.Single(await _userRepository.LoadAsync());
    Assert.Null(user.TenantId);
    Assert.Equal(user.EntityId.ToGuid(), session.User.Id);
  }

  [Fact(DisplayName = "It should create a realm session.")]
  public async Task It_should_create_a_realm_session()
  {
    SetRealm();

    User user = new(new UniqueName(Realm.UniqueNameSettings, UsernameString), actorId: null, UserId.NewId(TenantId));
    await _userRepository.SaveAsync(user);

    CreateSessionPayload payload = new(UsernameString);
    payload.CustomAttributes.Add(new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"));
    payload.CustomAttributes.Add(new("IpAddress", Faker.Internet.Ip()));
    CreateSessionCommand command = new(payload);
    SessionModel session = await ActivityPipeline.ExecuteAsync(command);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
    Assert.Equal(user.EntityId.ToGuid(), session.User.Id);
  }

  [Theory(DisplayName = "It should create a session to an user ID.")]
  [InlineData(null)]
  [InlineData("24122d4c-5dcf-4b85-b1fe-b9f7d5052726")]
  public async Task It_should_create_a_session_to_an_user_Id(string? idValue)
  {
    SetRealm();

    User user = new(new UniqueName(Realm.UniqueNameSettings, UsernameString), actorId: null, UserId.NewId(TenantId));
    await _userRepository.SaveAsync(user);

    CreateSessionPayload payload = new(user.EntityId.ToGuid().ToString());
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    CreateSessionCommand command = new(payload);
    SessionModel session = await ActivityPipeline.ExecuteAsync(command);

    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, session.Id);
    }
    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
    Assert.Equal(user.EntityId.ToGuid(), session.User.Id);
  }

  [Fact(DisplayName = "It should create a session to an user with its email as unique name.")]
  public async Task It_should_create_a_session_to_an_user_with_its_email_as_unique_name()
  {
    SetRealm();

    User user = new(new UniqueName(Realm.UniqueNameSettings, Faker.Person.Email), actorId: null, UserId.NewId(TenantId));
    user.SetEmail(new Email(Faker.Person.Email, isVerified: true));
    await _userRepository.SaveAsync(user);

    CreateSessionPayload payload = new(Faker.Person.Email);
    CreateSessionCommand command = new(payload);
    SessionModel session = await ActivityPipeline.ExecuteAsync(command);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
    Assert.Equal(user.EntityId.ToGuid(), session.User.Id);
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already taken.")]
  public async Task It_should_throw_IdAlreadyUsedException_when_the_Id_is_already_taken()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());

    Session session = new(user);
    await _sessionRepository.SaveAsync(session);

    CreateSessionPayload payload = new(user.EntityId.ToGuid().ToString())
    {
      Id = user.EntityId.ToGuid()
    };
    CreateSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Id.Value, exception.Id);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple users are found.")]
  public async Task It_should_throw_TooManyResultsException_when_multiple_users_are_found()
  {
    User user = (await _userRepository.LoadAsync()).Single();
    user.SetEmail(new Email(Faker.Person.Email, isVerified: true));
    User other = new(new UniqueName(new ReadOnlyUniqueNameSettings(), Faker.Person.Email));
    await _userRepository.SaveAsync([user, other]);

    CreateSessionPayload payload = new(Faker.Person.Email);
    CreateSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<User>>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user could not be found.")]
  public async Task It_should_throw_UserNotFoundException_when_the_user_could_not_be_found()
  {
    SetRealm();

    CreateSessionPayload payload = new(UsernameString);
    CreateSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(TenantId.ToGuid(), exception.TenantId);
    Assert.Equal(payload.User, exception.User);
    Assert.Equal("User", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateSessionPayload payload = new(user: string.Empty)
    {
      Id = Guid.Empty
    };
    CreateSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "User");
  }
}
