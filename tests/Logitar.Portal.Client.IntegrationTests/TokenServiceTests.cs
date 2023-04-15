using Bogus;
using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Client;

internal class TokenServiceTests
{
  private readonly Faker _faker = new();

  private readonly TestContext _context;
  private readonly ITokenService _tokenService;

  public TokenServiceTests(TestContext context, ITokenService tokenService)
  {
    _context = context;
    _tokenService = tokenService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      string realm = _context.Realm.Id.ToString();
      string locale = _context.Realm.DefaultLocale ?? string.Empty;

      name = string.Join('.', nameof(TokenService), nameof(TokenService.CreateAsync));
      CreateTokenInput create = new()
      {
        IsConsumable = true,
        Purpose = "reset_password",
        Realm = realm,
        Subject = Guid.NewGuid().ToString()
      };
      CreatedToken createdToken = await _tokenService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(TokenService), nameof(TokenService.ValidateAsync));
      ValidateTokenInput validate = new()
      {
        Token = createdToken.Token,
        Purpose = create.Purpose,
        Realm = create.Realm
      };
      _ = await _tokenService.ValidateAsync(validate, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(TokenService), nameof(TokenService.ConsumeAsync));
      _ = await _tokenService.ConsumeAsync(validate, cancellationToken);
      _context.Succeed(name);

      return true;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);

      return false;
    }
  }
}
