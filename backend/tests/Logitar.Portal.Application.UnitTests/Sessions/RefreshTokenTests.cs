using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Application.Sessions;

[Trait(Traits.Category, Categories.Unit)]
public class RefreshTokenTests
{
  [Fact(DisplayName = "ctor: it should construct the correct refresh token.")]
  public void ctor_it_should_construct_the_correct_refresh_token()
  {
    SessionId id = SessionId.NewId();
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    RefreshToken refreshToken = new(id, secret);
    Assert.Equal(id, refreshToken.Id);
    Assert.Equal(secret, refreshToken.Secret);
  }

  [Fact(DisplayName = "ctor: it should throw ArgumentException when the secret length is not exact.")]
  public void ctor_it_should_throw_ArgumentException_when_the_secret_length_is_not_exact()
  {
    SessionId id = SessionId.NewId();
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength * 2, out _);

    var exception = Assert.Throws<ArgumentException>(() => new RefreshToken(id, secret));
    Assert.StartsWith("The secret must contain exactly 32 bytes.", exception.Message);
    Assert.Equal("secret", exception.ParamName);
  }

  [Theory(DisplayName = "Decode: it should decode the correct refresh token.")]
  [InlineData(null)]
  [InlineData("0ed876c2-667c-4d8d-8356-f17ede5ecee5")]
  public void Decode_it_should_decode_the_correct_refresh_token(string? tenantIdValue)
  {
    TenantId? tenantId = tenantIdValue == null ? null : new(tenantIdValue);
    SessionId id = SessionId.NewId(tenantId);
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    string value = $"RT.{id.EntityId}.{secret.ToUriSafeBase64()}";

    RefreshToken refreshToken = RefreshToken.Decode(tenantId, value);
    Assert.Equal(id, refreshToken.Id);
    Assert.Equal(secret, refreshToken.Secret);
  }

  [Theory(DisplayName = "Decode: it should throw ArgumentException when the value is not valid.")]
  [InlineData("PT.abc.123")]
  [InlineData("abc.123")]
  public void Decode_it_should_throw_ArgumentException_when_the_value_is_not_valid(string value)
  {
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Decode(tenantId: null, value));
    Assert.StartsWith($"The value '{value}' is not a valid refresh token.", exception.Message);
    Assert.Equal("value", exception.ParamName);
  }

  [Fact(DisplayName = "Encode: it should encode correctly the refresh token.")]
  public void Encode_it_should_encode_correctly_the_refresh_token()
  {
    SessionId id = SessionId.NewId();
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    RefreshToken refreshToken = new(id, secret);

    string encoded = $"RT.{id.Value}.{secret.ToUriSafeBase64()}";
    Assert.Equal(encoded, refreshToken.Encode());
  }

  [Fact(DisplayName = "Encode: it should encode correctly the refresh token from parts.")]
  public void Encode_it_should_encode_correctly_the_refresh_token_from_parts()
  {
    SessionId id = SessionId.NewId();
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);

    string encoded = $"RT.{id.Value}.{secret.ToUriSafeBase64()}";
    Assert.Equal(encoded, RefreshToken.Encode(id, secret));
  }

  [Fact(DisplayName = "Encode: it should throw ArgumentException when the secret length is not exact.")]
  public void Encode_it_should_throw_ArgumentException_when_the_secret_length_is_not_exact()
  {
    SessionId id = SessionId.NewId();
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength * 2, out _);

    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Encode(id, secret));
    Assert.StartsWith("The secret must contain exactly 32 bytes.", exception.Message);
    Assert.Equal("secret", exception.ParamName);
  }
}
