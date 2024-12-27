using Logitar.Data;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;
using Logitar.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal.Application.Tokens.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ValidateTokenCommandTests : IntegrationTests
{
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  private readonly IBaseUrl _baseUrl;
  private readonly IConfigurationRepository _configurationRepository;

  public ValidateTokenCommandTests() : base()
  {
    _tokenHandler.InboundClaimTypeMap.Clear();

    _baseUrl = ServiceProvider.GetRequiredService<IBaseUrl>();
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.TokenBlacklist.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ValidateTokenPayload payload = new(string.Empty);
    ValidateTokenCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("Token", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should validate a custom token.")]
  public async Task It_should_validate_a_custom_token()
  {
    SetRealm();

    JwtSecret secret = JwtSecret.Generate(512 / 8);

    string tokenId = Guid.NewGuid().ToString();
    const string tokenType = "Custom+JWT";
    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = $"urn:logitar:portal:realm:unique-slug:{Realm.UniqueSlug}",
      Issuer = $"{_baseUrl.Value.TrimEnd('/')}/realms/{Realm.Id}",
      SigningCredentials = CreateSigningCredentials(secret.Value),
      Subject = new ClaimsIdentity([new Claim(Rfc7519ClaimNames.TokenId, tokenId), new Claim(Rfc7519ClaimNames.Username, Faker.Person.UserName)]),
      TokenType = tokenType
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    ValidateTokenPayload payload = new(token)
    {
      Consume = true,
      Audience = "urn:logitar:portal:realm:unique-slug:{UniqueSlug}",
      Issuer = "{BaseUrl}/realms/{Id}",
      Secret = secret.Value,
      Type = tokenType
    };
    ValidateTokenCommand command = new(payload);
    ValidatedTokenModel validatedToken = await ActivityPipeline.ExecuteAsync(command);
    Assert.Equal(Faker.Person.UserName, validatedToken.Claims.Single(x => x.Name == Rfc7519ClaimNames.Username).Value);

    BlacklistedTokenEntity? blacklisted = await IdentityContext.TokenBlacklist.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(blacklisted);
    Assert.Equal(tokenId, blacklisted.TokenId);
  }

  [Fact(DisplayName = "It should validate a Portal token.")]
  public async Task It_should_validate_a_Portal_token()
  {
    Configuration? configuration = await _configurationRepository.LoadAsync();
    Assert.NotNull(configuration);

    string baseUrl = _baseUrl.Value.TrimEnd('/');
    string subject = Guid.NewGuid().ToString();
    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = baseUrl,
      Issuer = baseUrl,
      SigningCredentials = CreateSigningCredentials(configuration.Secret.Value),
      Subject = new ClaimsIdentity([new Claim(Rfc7519ClaimNames.Subject, subject)])
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    ValidateTokenPayload payload = new(token);
    ValidateTokenCommand command = new(payload);
    ValidatedTokenModel validatedToken = await ActivityPipeline.ExecuteAsync(command);
    Assert.Equal(subject, validatedToken.Subject);
  }

  [Fact(DisplayName = "It should validate a realm token.")]
  public async Task It_should_validate_a_realm_token()
  {
    SetRealm();

    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = Realm.Url,
      Issuer = Realm.Url,
      SigningCredentials = CreateSigningCredentials(Realm.Secret),
      Subject = new ClaimsIdentity([new Claim(Rfc7519ClaimNames.EmailAddress, Faker.Person.Email), new Claim(Rfc7519ClaimNames.IsEmailVerified, true.ToString())])
    };
    SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
    string token = _tokenHandler.WriteToken(securityToken);

    ValidateTokenPayload payload = new(token);
    ValidateTokenCommand command = new(payload);
    ValidatedTokenModel validatedToken = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(validatedToken);
    Assert.NotNull(validatedToken.Email);
    Assert.Equal(Faker.Person.Email, validatedToken.Email.Address);
    Assert.True(validatedToken.Email.IsVerified);
  }

  private static SigningCredentials CreateSigningCredentials(string secret)
  {
    string algorithm = $"HS{secret.Length * 8}";
    return new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)), algorithm);
  }
}
