using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands
{
  internal class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, TokenModel>
  {
    private readonly IInternalTokenService _internalTokenService;

    public CreateTokenCommandHandler(IInternalTokenService internalTokenService)
    {
      _internalTokenService = internalTokenService;
    }

    public async Task<TokenModel> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
      return await _internalTokenService.CreateAsync(request.Payload, cancellationToken);
    }
  }
}
