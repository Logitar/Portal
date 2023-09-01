using Logitar.EventSourcing;
using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.ApiKeys;

[Trait(Traits.Category, Categories.Unit)]
public class ApiKeyAggregateTests
{
  private readonly string _secret;
  private readonly ApiKeyAggregate _apiKey;

  public ApiKeyAggregateTests()
  {
    _secret = RandomStringGenerator.GetString(ApiKeyAggregate.SecretLength);
    PasswordMock secret = new(_secret);
    _apiKey = new("Default", secret);
  }

  [Fact(DisplayName = "Authenticate: it should authenticate the API key.")]
  public void Authenticate_it_should_authenticate_the_Api_key()
  {
    ActorId actorId = ActorId.NewId();
    _apiKey.Authenticate(_secret, actorId);

    ApiKeyAuthenticatedEvent authenticated = (ApiKeyAuthenticatedEvent)Assert.Single(_apiKey.Changes, change => change is ApiKeyAuthenticatedEvent);
    Assert.Equal(actorId, authenticated.ActorId);
  }

  [Fact(DisplayName = "Authenticate: it should use the API key ID as actor ID.")]
  public void Authenticate_it_should_use_the_Api_key_Id_as_actor_Id()
  {
    ActorId actorId = new(_apiKey.Id.Value);
    _apiKey.Authenticate(_secret);

    ApiKeyAuthenticatedEvent authenticated = (ApiKeyAuthenticatedEvent)Assert.Single(_apiKey.Changes, change => change is ApiKeyAuthenticatedEvent);
    Assert.Equal(actorId, authenticated.ActorId);
  }

  [Fact(DisplayName = "Authenticate: it should throw ApiKeyIsExpiredException when it is expired.")]
  public void Authenticate_it_should_throw_ApiKeyIsExpiredException_when_it_is_expired()
  {
    _apiKey.ExpiresOn = DateTime.Now.AddMilliseconds(100);
    Thread.Sleep(TimeSpan.FromMilliseconds(200));

    var exception = Assert.Throws<ApiKeyIsExpiredException>(() => _apiKey.Authenticate(_secret));
    Assert.Equal(_apiKey.ToString(), exception.ApiKey);
  }

  [Fact(DisplayName = "Authenticate: it should throw IncorrectApiKeySecretException when the secret is not correct.")]
  public void Authenticate_it_should_throw_IncorrectApiKeySecretException_when_the_secret_is_not_correct()
  {
    string secret = _secret[1..];
    var exception = Assert.Throws<IncorrectApiKeySecretException>(() => _apiKey.Authenticate(secret));
    Assert.Equal(_apiKey.ToString(), exception.ApiKey);
    Assert.Equal(secret, exception.Secret);
  }

  [Fact(DisplayName = "IsExpired: it should return true when the moment is equal to the expiration.")]
  public void IsExpired_it_should_return_true_when_the_moment_is_equal_to_the_expiration()
  {
    _apiKey.ExpiresOn = DateTime.Now.AddYears(1);
    Assert.True(_apiKey.IsExpired(_apiKey.ExpiresOn));
  }

  [Fact(DisplayName = "IsExpired: it should return true when the moment is greater than the expiration.")]
  public void IsExpired_it_should_return_true_when_the_moment_is_greater_than_the_expiration()
  {
    _apiKey.ExpiresOn = DateTime.Now.AddYears(1);
    Assert.True(_apiKey.IsExpired(DateTime.Now.AddYears(2)));
  }

  [Fact(DisplayName = "IsExpired: it should return false when it has no expiration.")]
  public void IsExpired_it_should_return_false_when_it_has_no_expiration()
  {
    Assert.False(_apiKey.IsExpired());
  }

  [Fact(DisplayName = "IsExpired: it should return false When the moment is less than the expiration.")]
  public void IsExpired_it_should_return_false_When_the_moment_is_less_than_the_expiration()
  {
    _apiKey.ExpiresOn = DateTime.Now.AddYears(1);
    Assert.False(_apiKey.IsExpired());
  }

  [Fact(DisplayName = "ExpiresOn: it should set the expiration date when current value is null.")]
  public void ExpiresOn_it_should_set_the_expiration_date_when_current_value_is_null()
  {
    DateTime expiresOn = DateTime.Now.AddYears(1);
    _apiKey.ExpiresOn = expiresOn;
    Assert.Equal(expiresOn, _apiKey.ExpiresOn);
  }

  [Fact(DisplayName = "ExpiresOn: it should set the expiration date when new value is less than current value.")]
  public void ExpiresOn_it_should_set_the_expiration_date_when_new_value_is_less_than_current_value()
  {
    _apiKey.ExpiresOn = DateTime.Now.AddYears(1);

    DateTime expiresOn = DateTime.Now.AddMonths(6);
    _apiKey.ExpiresOn = expiresOn;
    Assert.Equal(expiresOn, _apiKey.ExpiresOn);
  }

  [Fact(DisplayName = "ExpiresOn: it should throw CannotPostponeExpirationException when new value is greater than current value.")]
  public void ExpiresOn_it_should_throw_CannotPostponeExpirationException_when_new_value_is_greater_than_current_value()
  {
    _apiKey.ExpiresOn = DateTime.Now.AddMonths(1);

    DateTime expiresOn = DateTime.Now.AddYears(1);
    var exception = Assert.Throws<CannotPostponeExpirationException>(() => _apiKey.ExpiresOn = expiresOn);
    Assert.Equal(_apiKey.ToString(), exception.ApiKey);
    Assert.Equal(expiresOn, exception.ExpiresOn);
    Assert.Equal(nameof(_apiKey.ExpiresOn), exception.PropertyName);
  }

  [Fact(DisplayName = "ExpiresOn: it should throw CannotPostponeExpirationException when new value is null.")]
  public void ExpiresOn_it_should_throw_CannotPostponeExpirationException_when_new_value_is_null()
  {
    _apiKey.ExpiresOn = DateTime.Now.AddMonths(1);

    var exception = Assert.Throws<CannotPostponeExpirationException>(() => _apiKey.ExpiresOn = null);
    Assert.Equal(_apiKey.ToString(), exception.ApiKey);
    Assert.Null(exception.ExpiresOn);
    Assert.Equal(nameof(_apiKey.ExpiresOn), exception.PropertyName);
  }
}
