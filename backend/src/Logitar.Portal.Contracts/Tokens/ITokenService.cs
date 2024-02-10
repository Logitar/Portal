namespace Logitar.Portal.Contracts.Tokens;

public interface ITokenService
{
  Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken = default);
}
