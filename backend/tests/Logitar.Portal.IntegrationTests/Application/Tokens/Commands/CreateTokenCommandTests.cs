using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;
using Logitar.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal.Application.Tokens.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateTokenCommandTests : IntegrationTests
{
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  private readonly IBaseUrl _baseUrl;
  private readonly IConfigurationRepository _configurationRepository;

  public CreateTokenCommandTests() : base()
  {
    _tokenHandler.InboundClaimTypeMap.Clear();

    _baseUrl = ServiceProvider.GetRequiredService<IBaseUrl>();
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
  }

  [Fact(DisplayName = "It should create a custom token.")]
  public async Task It_should_create_a_custom_token()
  {
    SetRealm();

    JwtSecret secret = JwtSecret.Generate(length: 512 / 8);
    CreateTokenPayload payload = new()
    {
      Algorithm = "HS512",
      Audience = "{Url}",
      Issuer = "{BaseUrl}/realms/{Id}",
      LifetimeSeconds = 24 * 60 * 60,
      Secret = secret.Value,
      Type = "Custom+JWT"
    };
    CreateTokenCommand command = new(payload);
    CreatedTokenModel createdToken = await ActivityPipeline.ExecuteAsync(command);

    TokenValidationParameters validationParameters = new()
    {
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret.Value)),
      ValidAudience = Realm.Url,
      ValidIssuer = $"{_baseUrl.Value.TrimEnd('/')}/realms/{Realm.Id}",
      ValidTypes = [payload.Type],
      ValidateAudience = true,
      ValidateIssuer = true,
      ValidateIssuerSigningKey = true
    };
    ClaimsPrincipal principal = _tokenHandler.ValidateToken(createdToken.Token, validationParameters, out _);
    DateTime expires = ClaimHelper.ExtractDateTime(principal.FindAll(Rfc7519ClaimNames.ExpirationTime).Single());
    Assert.Equal(DateTime.UtcNow.AddSeconds(payload.LifetimeSeconds.Value), expires, TimeSpan.FromMinutes(1));
  }

  [Fact(DisplayName = "It should create a Portal token.")]
  public async Task It_should_create_a_Portal_token()
  {
    Configuration? configuration = await _configurationRepository.LoadAsync();
    Assert.NotNull(configuration);

    string subject = Guid.NewGuid().ToString();
    CreateTokenPayload payload = new()
    {
      Subject = subject,
      Email = new EmailPayload(Faker.Person.Email, isVerified: true)
    };
    CreateTokenCommand command = new(payload);
    CreatedTokenModel createdToken = await ActivityPipeline.ExecuteAsync(command);

    string baseUrl = _baseUrl.Value.TrimEnd('/');
    TokenValidationParameters validationParameters = new()
    {
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.Secret.Value)),
      ValidAudience = baseUrl,
      ValidIssuer = baseUrl,
      ValidateAudience = true,
      ValidateIssuer = true,
      ValidateIssuerSigningKey = true
    };
    ClaimsPrincipal principal = _tokenHandler.ValidateToken(createdToken.Token, validationParameters, out _);
    Assert.DoesNotContain(principal.Claims, c => c.Type == Rfc7519ClaimNames.TokenId);
    Assert.Equal(subject, principal.FindAll(Rfc7519ClaimNames.Subject).Single().Value);
    Assert.Equal(Faker.Person.Email, principal.FindAll(Rfc7519ClaimNames.EmailAddress).Single().Value);
    Assert.Equal(true.ToString(), principal.FindAll(Rfc7519ClaimNames.IsEmailVerified).Single().Value);
  }

  [Fact(DisplayName = "It should create a realm token.")]
  public async Task It_should_create_a_realm_token()
  {
    Realm.Url = null;
    SetRealm();

    CreateTokenPayload payload = new()
    {
      IsConsumable = true,
    };
    payload.Claims.Add(new(Rfc7519ClaimNames.FirstName, Faker.Person.FirstName));
    payload.Claims.Add(new(Rfc7519ClaimNames.LastName, Faker.Person.LastName));
    CreateTokenCommand command = new(payload);
    CreatedTokenModel createdToken = await ActivityPipeline.ExecuteAsync(command);

    TokenValidationParameters validationParameters = new()
    {
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Realm.Secret)),
      ValidAudience = Realm.UniqueSlug,
      ValidIssuer = $"{_baseUrl.Value.TrimEnd('/')}/realms/unique-slug:{Realm.UniqueSlug}",
      ValidateAudience = true,
      ValidateIssuer = true,
      ValidateIssuerSigningKey = true
    };
    ClaimsPrincipal principal = _tokenHandler.ValidateToken(createdToken.Token, validationParameters, out _);
    Assert.Contains(principal.Claims, c => c.Type == Rfc7519ClaimNames.TokenId);
    Assert.Equal(Faker.Person.FirstName, principal.FindAll(Rfc7519ClaimNames.FirstName).Single().Value);
    Assert.Equal(Faker.Person.LastName, principal.FindAll(Rfc7519ClaimNames.LastName).Single().Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateTokenPayload payload = new()
    {
      LifetimeSeconds = -60
    };
    CreateTokenCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("LifetimeSeconds.Value", exception.Errors.Single().PropertyName);
  }
}
