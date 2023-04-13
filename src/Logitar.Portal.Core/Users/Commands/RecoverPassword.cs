using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record RecoverPassword(RecoverPasswordInput Input) : IRequest<SentMessages>;
