using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record VerifyAddress(Guid Id) : IRequest<User>;
