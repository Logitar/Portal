﻿using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Application.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignInSessionCommandTests : IntegrationTests
{
  public SignInSessionCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a persistent session.")]
  public async Task It_should_create_a_persistent_session()
  {
    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
      new("IpAddress", Faker.Internet.Ip())
    ];
    SignInSessionPayload payload = new(Faker.Person.UserName, PasswordString, isPersistent: true, customAttributes);
    SignInSessionCommand command = new(payload);
    Session session = await Mediator.Send(command);

    Assert.True(session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(customAttributes, session.CustomAttributes);
    Assert.Equal(Faker.Person.UserName, session.User.UniqueName);

    Assert.NotNull(session.RefreshToken);
    RefreshToken refreshToken = RefreshToken.Decode(session.RefreshToken);
    Assert.Equal(session.Id, refreshToken.Id.AggregateId.ToGuid());
    Assert.Equal(RefreshToken.SecretLength, Convert.FromBase64String(refreshToken.Secret).Length);
  }

  [Fact(DisplayName = "It should create an ephemereal session.")]
  public async Task It_should_create_an_ephemereal_session()
  {
    SignInSessionPayload payload = new(Faker.Person.UserName, PasswordString);
    SignInSessionCommand command = new(payload);
    Session session = await Mediator.Send(command);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Empty(session.CustomAttributes);
    Assert.Equal(Faker.Person.UserName, session.User.UniqueName);
  }

  [Fact(DisplayName = "It should throw IncorrectUserPasswordException when the password is incorrect.")]
  public async Task It_should_throw_IncorrectUserPasswordException_when_the_password_is_incorrect()
  {
    SignInSessionPayload payload = new(Faker.Person.UserName, PasswordString[..^1]);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(async () => await Mediator.Send(command));
    Assert.Equal(payload.Password, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user could not be found.")]
  public async Task It_should_throw_UserNotFoundException_when_the_user_could_not_be_found()
  {
    Realm realm = new("tests", JwtSecretUnit.Generate().Value)
    {
      Id = Guid.NewGuid()
    };
    ApplicationContext.Realm = realm;

    SignInSessionPayload payload = new(Faker.Person.UserName, PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(new AggregateId(realm.Id).Value, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SignInSessionPayload payload = new(Faker.Person.UserName, password: string.Empty);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("Password", exception.Errors.Single().PropertyName);
  }
}
