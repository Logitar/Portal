using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record VerifyPhone(Guid Id) : IRequest<User>;
