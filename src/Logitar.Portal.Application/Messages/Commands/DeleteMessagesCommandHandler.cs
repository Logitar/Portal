using Logitar.Portal.Domain.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

internal class DeleteMessagesCommandHandler : INotificationHandler<DeleteMessagesCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMessageRepository _messageRepository;

  public DeleteMessagesCommandHandler(IApplicationContext applicationContext, IMessageRepository messageRepository)
  {
    _applicationContext = applicationContext;
    _messageRepository = messageRepository;
  }

  public async Task Handle(DeleteMessagesCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<MessageAggregate> messages = await _messageRepository.LoadAsync(command.Realm, cancellationToken);
    foreach (MessageAggregate message in messages)
    {
      message.Delete(_applicationContext.ActorId);
    }

    await _messageRepository.SaveAsync(messages, cancellationToken);
  }
}
