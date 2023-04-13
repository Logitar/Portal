using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

internal record SendMessage(SendMessageInput Input) : IRequest<SentMessages>;
