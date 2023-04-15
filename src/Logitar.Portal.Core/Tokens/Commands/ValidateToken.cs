using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Core.Tokens.Commands;

internal record ValidateToken(ValidateTokenInput Input, bool Consume) : IRequest<ValidatedToken>;
