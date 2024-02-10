using Logitar.Portal.Application.Tokens.Commands;
using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Application.Tokens;

internal class TokenFacade : ITokenService
{
  private readonly IMediator _mediator;

  public TokenFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<CreatedToken> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateTokenCommand(payload), cancellationToken);
  }
}
