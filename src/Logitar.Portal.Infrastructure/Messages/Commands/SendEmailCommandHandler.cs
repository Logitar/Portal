using Logitar.Portal.Application.Messages.Commands;
using MediatR;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class SendEmailCommandHandler : INotificationHandler<SendEmailCommand>
{
  public Task Handle(SendEmailCommand command, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
