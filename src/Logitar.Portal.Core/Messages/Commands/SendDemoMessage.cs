using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

internal record SendDemoMessage(SendDemoMessageInput Input) : IRequest<Message>;
