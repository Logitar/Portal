using Logitar.Portal.v2.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Commands;

internal record SendDemoMessage(SendDemoMessageInput Input) : IRequest<Message>;
