using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Logitar.Security.Claims;
using Logitar.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class TokenServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITokenBlacklist _tokenBlacklist;
  private readonly ITokenManager _tokenManager;
  private readonly ITokenService _tokenService;

  private readonly RealmAggregate _realm;

  public TokenServiceTests() : base()
  {
    _applicationContext = ServiceProvider.GetRequiredService<IApplicationContext>();
    _tokenBlacklist = ServiceProvider.GetRequiredService<ITokenBlacklist>();
    _tokenManager = ServiceProvider.GetRequiredService<ITokenManager>();
    _tokenService = ServiceProvider.GetRequiredService<ITokenService>();

    _realm = new("desjardins", requireUniqueEmail: true, requireConfirmedAccount: true)
    {
      DisplayName = "Desjardins",
      DefaultLocale = Locale,
      Url = new Uri("https://www.desjardins.com/")
    };
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(_realm);
  }

  [Fact(DisplayName = "CreateAsync: it should create a Portal token.")]
  public async Task CreateAsync_it_should_create_a_Portal_token()
  {
    CreateTokenPayload payload = new()
    {
      Subject = Guid.NewGuid().ToString()
    };

    CreatedToken createdToken = await _tokenService.CreateAsync(payload);

    Assert.NotNull(Configuration);
    string secret = Configuration.Secret;
    ClaimsPrincipal principal = _tokenManager.Validate(createdToken.Token, secret);
    Assert.Equal(6, principal.Claims.Count());
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.Audience && claim.Value == _applicationContext.BaseUrl?.ToString());
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.ExpirationTime && long.TryParse(claim.Value, out _));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.IssuedAt && long.TryParse(claim.Value, out _));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.Issuer && claim.Value == _applicationContext.BaseUrl?.ToString());
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.NotBefore && long.TryParse(claim.Value, out _));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.Subject && claim.Value == payload.Subject);
  }

  [Fact(DisplayName = "CreateAsync: it should create a realm token.")]
  public async Task CreateAsync_it_should_create_a_realm_token()
  {
    CreateTokenPayload payload = new()
    {
      IsConsumable = true,
      Realm = $"  {_realm.UniqueSlug.ToUpper()}  ",
      Algorithm = SecurityAlgorithms.HmacSha512,
      Audience = $"  {{Url}}oauth2  ",
      Lifetime = 7 * 24 * 60 * 60,
      Type = "  createuser+jwt  ",
      Secret = "k_C!W9{f-(w57>LU+p:&<ZXtjSV@h$8;K#PyTmrFsM?bz[uQ`=RYd~.v*N]^ED6}",
      EmailAddress = $"  {Faker.Person.Email}  ",
      Claims = new Contracts.Tokens.Claim[]
      {
        new(Rfc7519ClaimNames.IsEmailVerified, "true", ClaimValueTypes.Boolean)
      }
    };

    CreatedToken createdToken = await _tokenService.CreateAsync(payload);

    ClaimsPrincipal principal = _tokenManager.Validate(createdToken.Token, payload.Secret, type: payload.Type.Trim());
    Assert.Equal(8, principal.Claims.Count());
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.Audience
      && claim.Value == "https://www.desjardins.com/oauth2");
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.EmailAddress
      && claim.Value == payload.EmailAddress.Trim());
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.ExpirationTime
      && long.TryParse(claim.Value, out _));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.IsEmailVerified
      && claim.Value == "true");
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.IssuedAt
      && long.TryParse(claim.Value, out _));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.Issuer
      && claim.Value == $"{_applicationContext.BaseUrl?.ToString().TrimEnd('/')}/realms/{_realm.UniqueSlug}");
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.NotBefore
      && long.TryParse(claim.Value, out _));
    Assert.Contains(principal.Claims, claim => claim.Type == Rfc7519ClaimNames.TokenId
      && Guid.TryParse(claim.Value, out _));

    System.Security.Claims.Claim claim = Assert.Single(principal.FindAll(Rfc7519ClaimNames.ExpirationTime));
    long seconds = long.Parse(claim.Value);
    DateTime expiresOn = DateTime.SpecifyKind(DateTimeOffset.FromUnixTimeSeconds(seconds).DateTime, DateTimeKind.Utc).AddSeconds(-payload.Lifetime);
    AssertIsNear(expiresOn);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the realm could not be found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_realm_could_not_be_found()
  {
    CreateTokenPayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _tokenService.CreateAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw ValidationException when the email address is not valid.")]
  public async Task CreateAsync_it_should_throw_ValidationException_when_the_email_address_is_not_valid()
  {
    CreateTokenPayload payload = new()
    {
      EmailAddress = "aa@@bb..cc"
    };

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _tokenService.CreateAsync(payload));
    Assert.Contains(exception.Errors, error => error.ErrorCode == "EmailValidator" && error.PropertyName == "Address");
  }

  [Fact(DisplayName = "CreateAsync: it should throw ValidationException when the secret is not valid.")]
  public async Task CreateAsync_it_should_throw_ValidationException_when_the_secret_is_not_valid()
  {
    CreateTokenPayload payload = new()
    {
      Secret = RandomStringGenerator.GetString(128 / 8)
    };

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _tokenService.CreateAsync(payload));
    Assert.Contains(exception.Errors, error => error.ErrorCode == "MinimumLengthValidator" && error.PropertyName == "Secret");
  }

  [Fact(DisplayName = "ValidateAsync: it should consume a consumable token.")]
  public async Task ValidateAsync_it_should_consume_a_consumable_token()
  {
    CreateTokenPayload createPayload = new()
    {
      IsConsumable = true
    };
    CreatedToken createdToken = await _tokenService.CreateAsync(createPayload);

    ValidateTokenPayload validatePayload = new(createdToken.Token)
    {
      Consume = true
    };
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(validatePayload);

    Contracts.Tokens.Claim jti = Assert.Single(validatedToken.Claims, claim => claim.Name == Rfc7519ClaimNames.TokenId);
    Guid id = Guid.Parse(jti.Value);

    Contracts.Tokens.Claim exp = Assert.Single(validatedToken.Claims, claim => claim.Name == Rfc7519ClaimNames.ExpirationTime);
    DateTime expiresOn = DateTime.SpecifyKind(DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp.Value)).DateTime, DateTimeKind.Utc);

    BlacklistedTokenEntity? blacklisted = await PortalContext.TokenBlacklist.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
    Assert.NotNull(blacklisted);
    Assert.Equal(id, blacklisted.Id);
    Assert.Equal(expiresOn.AddHours(1), blacklisted.ExpiresOn);
  }

  [Fact(DisplayName = "ValidateAsync: it should not consume a non-consumable token.")]
  public async Task ValidateAsync_it_should_not_consume_a_non_consumable_token()
  {
    Assert.Empty(await PortalContext.TokenBlacklist.AsNoTracking().ToArrayAsync());

    CreateTokenPayload createPayload = new()
    {
      Subject = Guid.NewGuid().ToString()
    };
    CreatedToken createdToken = await _tokenService.CreateAsync(createPayload);

    ValidateTokenPayload validatePayload = new(createdToken.Token)
    {
      Consume = true
    };
    _ = await _tokenService.ValidateAsync(validatePayload);

    Assert.Empty(await PortalContext.TokenBlacklist.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "ValidateAsync: it should throw AggregateNotFoundException when the realm could not be found.")]
  public async Task ValidateAsync_it_should_throw_AggregateNotFoundException_when_the_realm_could_not_be_found()
  {
    ValidateTokenPayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _tokenService.ValidateAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "ValidateAsync: it should throw SecurityTokenBlacklistedException when the token is blacklisted.")]
  public async Task ValidateAsync_it_should_throw_SecurityTokenBlacklistedException_when_the_token_is_blacklisted()
  {
    Guid blacklistedId = Guid.NewGuid();
    await _tokenBlacklist.BlacklistAsync(new[] { blacklistedId });

    ClaimsIdentity identity = new();
    identity.AddClaim(new(Rfc7519ClaimNames.Audience, _realm.Url!.ToString()));
    identity.AddClaim(new(Rfc7519ClaimNames.Issuer, $"{_applicationContext.BaseUrl}realms/{_realm.UniqueSlug}"));
    identity.AddClaim(new(Rfc7519ClaimNames.TokenId, blacklistedId.ToString()));
    identity.AddClaim(new(Rfc7519ClaimNames.TokenId, Guid.NewGuid().ToString()));

    string token = _tokenManager.Create(identity, _realm.Secret);

    ValidateTokenPayload payload = new(token)
    {
      Realm = $"  {_realm.UniqueSlug.ToUpper()}  "
    };

    var exception = await Assert.ThrowsAsync<SecurityTokenBlacklistedException>(async () => await _tokenService.ValidateAsync(payload));
    Assert.Equal(new[] { blacklistedId }, exception.BlacklistedIds);
  }

  [Fact(DisplayName = "ValidateAsync: it should throw ValidationException when the secret is not valid.")]
  public async Task ValidateAsync_it_should_throw_ValidationException_when_the_secret_is_not_valid()
  {
    ValidateTokenPayload payload = new()
    {
      Secret = RandomStringGenerator.GetString(128 / 8)
    };

    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _tokenService.ValidateAsync(payload));
    Assert.Contains(exception.Errors, error => error.ErrorCode == "MinimumLengthValidator" && error.PropertyName == "Secret");
  }

  [Fact(DisplayName = "ValidateAsync: it should validate a Portal token.")]
  public async Task ValidateAsync_it_should_validate_a_Portal_token()
  {
    CreateTokenPayload createPayload = new()
    {
      Subject = $"  {Guid.NewGuid()}  ",
      EmailAddress = $"  {Faker.Person.Email}  ",
      Claims = new Contracts.Tokens.Claim[]
      {
        new(Rfc7519ClaimNames.FullName, Faker.Person.FullName, "    "),
        new($"  {Rfc7519ClaimNames.IsEmailVerified}  ", "  true  ", $"  {ClaimValueTypes.Boolean}  ")
      }
    };
    CreatedToken createdToken = await _tokenService.CreateAsync(createPayload);

    ValidateTokenPayload validatePayload = new(createdToken.Token)
    {
      Consume = true
    };
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(validatePayload);

    Assert.Equal(createPayload.Subject.Trim(), validatedToken.Subject);
    Assert.Equal(createPayload.EmailAddress.Trim(), validatedToken.EmailAddress);

    Assert.Contains(validatedToken.Claims, claim => claim.Name == Rfc7519ClaimNames.FullName
      && claim.Value == Faker.Person.FullName && claim.Type == ClaimValueTypes.String);
    Assert.Contains(validatedToken.Claims, claim => claim.Name == Rfc7519ClaimNames.IsEmailVerified
      && claim.Value == "true" && claim.Type == ClaimValueTypes.Boolean);
  }

  [Fact(DisplayName = "ValidateAsync: it should validate a realm token.")]
  public async Task ValidateAsync_it_should_validate_a_realm_token()
  {
    CreateTokenPayload createPayload = new()
    {
      Realm = _realm.UniqueSlug,
      Type = "createuser+jwt",
      Claims = new Contracts.Tokens.Claim[]
      {
        new(Rfc7519ClaimNames.Username, Faker.Person.UserName),
        new(Rfc7519ClaimNames.EmailAddress, Faker.Person.Email),
        new(Rfc7519ClaimNames.FirstName, Faker.Person.FirstName),
        new(Rfc7519ClaimNames.LastName, Faker.Person.LastName)
      }
    };
    CreatedToken createdToken = await _tokenService.CreateAsync(createPayload);

    ValidateTokenPayload validatePayload = new(createdToken.Token)
    {
      Consume = true,
      Realm = _realm.Id.ToGuid().ToString(),
      Type = createPayload.Type
    };
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(validatePayload);

    Assert.Equal(Faker.Person.Email, validatedToken.EmailAddress);
    Assert.Contains(validatedToken.Claims, claim => claim.Name == Rfc7519ClaimNames.Username && claim.Value == Faker.Person.UserName);
    Assert.Contains(validatedToken.Claims, claim => claim.Name == Rfc7519ClaimNames.FirstName && claim.Value == Faker.Person.FirstName);
    Assert.Contains(validatedToken.Claims, claim => claim.Name == Rfc7519ClaimNames.LastName && claim.Value == Faker.Person.LastName);
  }
}
