using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands
{
  internal class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, ValidatedTokenModel>
  {
    private readonly IInternalTokenService _internalTokenService;

    public ValidateTokenCommandHandler(IInternalTokenService internalTokenService)
    {
      _internalTokenService = internalTokenService;
    }

    public async Task<ValidatedTokenModel> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
    {
      return await _internalTokenService.ValidateAsync(request.Payload, request.Consume, cancellationToken);
    }
  }
}
