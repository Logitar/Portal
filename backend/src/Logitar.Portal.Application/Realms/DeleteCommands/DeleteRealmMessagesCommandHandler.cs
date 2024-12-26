using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Messages;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal class DeleteRealmMessagesCommandHandler : INotificationHandler<DeleteRealmMessagesCommand>
{
  private readonly IMessageRepository _messageRepository;

  public DeleteRealmMessagesCommandHandler(IMessageRepository messageRepository)
  {
    _messageRepository = messageRepository;
  }

  public async Task Handle(DeleteRealmMessagesCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<Message> messages = await _messageRepository.LoadAsync(tenantId, cancellationToken);

    foreach (Message message in messages)
    {
      message.Delete(command.ActorId);
    }

    await _messageRepository.SaveAsync(messages, cancellationToken);
  }
}
