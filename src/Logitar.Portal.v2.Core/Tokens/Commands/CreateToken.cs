using Logitar.Portal.v2.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.v2.Core.Tokens.Commands;

internal record CreateToken(CreateTokenInput Input) : IRequest<CreatedToken>;
