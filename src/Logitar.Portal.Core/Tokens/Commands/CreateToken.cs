using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Core.Tokens.Commands;

internal record CreateToken(CreateTokenInput Input) : IRequest<CreatedToken>;
