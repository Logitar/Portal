using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record VerifyEmail(Guid Id) : IRequest<User>;
