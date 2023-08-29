using Bogus;
using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Sessions;

[Trait(Traits.Category, Categories.Unit)]
public class SessionAggregateTests
{
  private readonly Faker _faker = new();

  private readonly UserAggregate _user;
  private readonly string _secret;
  private readonly SessionAggregate _session;

  public SessionAggregateTests()
  {
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    _user = new(uniqueNameSettings, _faker.Person.UserName);

    _secret = RandomStringGenerator.GetString(SessionAggregate.SecretLength);
    PasswordMock secret = new(_secret);
    _session = new(_user, secret);
  }

  [Fact(DisplayName = "Renew: it should renew the session with a secret.")]
  public void Renew_it_should_renew_the_session_with_a_secret()
  {
    string secretString = RandomStringGenerator.GetString(SessionAggregate.SecretLength);
    PasswordMock newSecret = new(secretString);
    ActorId actorId = ActorId.NewId();

    _session.Renew(_secret, newSecret, actorId);

    Assert.Equal(actorId, _session.UpdatedBy);
    Assert.True(_session.CreatedOn < _session.UpdatedOn);
    Assert.True(_session.Version > 1);

    FieldInfo? secretField = _session.GetType().GetField("_secret", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(secretField);
    Password? secret = secretField.GetValue(_session) as Password;
    Assert.NotNull(secret);
    Assert.True(secret.IsMatch(secretString));
  }

  [Fact(DisplayName = "Renew: it should renew the session without a secret.")]
  public void Renew_it_should_renew_the_session_without_a_secret()
  {
    ActorId actorId = new(_user.Id.Value);

    _session.Renew(_secret);

    Assert.Equal(actorId, _session.UpdatedBy);
    Assert.True(_session.CreatedOn < _session.UpdatedOn);
    Assert.True(_session.Version > 1);

    FieldInfo? secretField = _session.GetType().GetField("_secret", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(secretField);
    Password? secret = secretField.GetValue(_session) as Password;
    Assert.Null(secret);
  }

  [Fact(DisplayName = "Renew: it should throw IncorrectSessionSecretException when secret is incorrect.")]
  public void Renew_it_should_throw_IncorrectSessionSecretException_when_secret_is_incorrect()
  {
    string secret = _secret[1..];
    var exception = Assert.Throws<IncorrectSessionSecretException>(() => _session.Renew(secret));
    Assert.Equal(_session.ToString(), exception.Session);
    Assert.Equal(secret, exception.Secret);
  }

  [Fact(DisplayName = "Renew: it should throw IncorrectSessionSecretException when session has no secret.")]
  public void Renew_it_should_throw_IncorrectSessionSecretException_when_session_has_no_secret()
  {
    SessionAggregate session = new(_user);
    var exception = Assert.Throws<IncorrectSessionSecretException>(() => session.Renew(_secret));
    Assert.Equal(session.ToString(), exception.Session);
    Assert.Equal(_secret, exception.Secret);
  }

  [Fact(DisplayName = "Renew: it should throw SessionIsNotActiveException when the session is not active.")]
  public void Renew_it_should_throw_SessionIsNotActiveException_when_the_session_is_not_active()
  {
    PropertyInfo? isActiveProperty = _session.GetType().GetProperty("IsActive");
    Assert.NotNull(isActiveProperty);
    isActiveProperty.SetValue(_session, false);

    var exception = Assert.Throws<SessionIsNotActiveException>(() => _session.Renew(_secret));
    Assert.Equal(_session.ToString(), exception.Session);
  }
}
