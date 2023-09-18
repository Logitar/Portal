using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

internal class SendDemoMessageCommandHandler : IRequestHandler<SendDemoMessageCommand, Message>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IMessageQuerier _messageQuerier;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  public SendDemoMessageCommandHandler(IApplicationContext applicationContext, IMediator mediator,
    IMessageQuerier messageQuerier, ITemplateRepository templateRepository, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _mediator = mediator;
    _messageQuerier = messageQuerier;
    _templateRepository = templateRepository;
    _userRepository = userRepository;
  }

  public async Task<Message> Handle(SendDemoMessageCommand command, CancellationToken cancellationToken)
  {
    AggregateId userId = new(_applicationContext.ActorId.Value);
    UserAggregate user = await _userRepository.LoadAsync(userId, version: null, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(userId, nameof(_applicationContext.ActorId));

    SendDemoMessagePayload payload = command.Payload;

    TemplateAggregate template = await _templateRepository.LoadAsync(payload.TemplateId, cancellationToken)
      ?? throw new AggregateNotFoundException<TemplateAggregate>(payload.TemplateId, nameof(payload.TemplateId));

    SendMessagePayload sendMessage = new()
    {
      Realm = template.TenantId == null ? null : new AggregateId(template.TenantId).ToGuid().ToString(),
      SenderId = payload.SenderId,
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = user.Id.ToGuid().ToString()
        }
      },
      IgnoreUserLocale = !string.IsNullOrWhiteSpace(payload.Locale),
      Locale = payload.Locale,
      Variables = payload.Variables
    };
    SentMessages sentMessages = await _mediator.Send(new SendMessageCommand(sendMessage, IsDemo: true, Realm: null, template, user), cancellationToken);

    Guid messageId = sentMessages.Ids.Single();
    return await _messageQuerier.ReadAsync(messageId, cancellationToken)
      ?? throw new InvalidOperationException($"The message 'Id={messageId}' could not be found.");
  }
}
