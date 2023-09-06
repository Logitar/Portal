using Bogus;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Client;

internal class TokenClientTests
{
  private const string Sut = "TokenClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly ITokenService _tokenService;

  public TokenClientTests(TestContext context, Faker faker, ITokenService tokenService)
  {
    _context = context;
    _faker = faker;
    _tokenService = tokenService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_tokenService.CreateAsync)}";
      CreateTokenPayload create = new()
      {
        IsConsumable = true,
        Purpose = "ResetPassword",
        Realm = _context.Realm.UniqueSlug,
        Lifetime = 900, // 15 minutes
        Subject = _context.User.Id.ToString(),
        EmailAddress = _context.User.Email?.Address,
        Claims = new Claim[]
        {
          new("email_verified", "true", "boolean")
        }
      };
      CreatedToken createdToken = await _tokenService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_tokenService.CreateAsync)}";
      ValidateTokenPayload validate = new()
      {
        Token = createdToken.Token,
        Consume = true,
        Purpose = create.Purpose,
        Realm = create.Realm
      };
      _ = await _tokenService.ValidateAsync(validate, cancellationToken);
      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
