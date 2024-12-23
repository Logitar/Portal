using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Messages.Validators;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

public record SendMessageInternalCommand(SendMessagePayload Payload) : Activity, IRequest<SentMessages>;

internal class SendMessageInternalCommandHandler : IRequestHandler<SendMessageInternalCommand, SentMessages>
{
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IMessageRepository _messageRepository;
  private readonly ISender _sender;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  public SendMessageInternalCommandHandler(IDictionaryRepository dictionaryRepository, IMessageRepository messageRepository, ISender sender,
    ISenderRepository senderRepository, ITemplateRepository templateRepository, IUserRepository userRepository)
  {
    _dictionaryRepository = dictionaryRepository;
    _messageRepository = messageRepository;
    _sender = sender;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
    _userRepository = userRepository;
  }

  public async Task<SentMessages> Handle(SendMessageInternalCommand command, CancellationToken cancellationToken)
  {
    ActorId actorId = command.ActorId;
    TenantId? tenantId = command.TenantId;
    Locale? defaultLocale = command.DefaultLocale;

    SendMessagePayload payload = command.Payload;
    Sender sender = await ResolveSenderAsync(tenantId, payload, cancellationToken);
    new SendMessageValidator(sender.Type).ValidateAndThrow(payload);

    Recipients allRecipients = await ResolveRecipientsAsync(tenantId, payload, sender.Type, cancellationToken);
    Template template = await ResolveTemplateAsync(tenantId, payload, sender.Type, cancellationToken);

    Dictionary<Locale, Dictionary> allDictionaries = (await _dictionaryRepository.LoadAsync(tenantId, cancellationToken))
      .ToDictionary(x => x.Locale, x => x);
    bool ignoreUserLocale = payload.IgnoreUserLocale;
    Locale? targetLocale = Locale.TryCreate(payload.Locale);
    Dictionaries defaultDictionaries = new(allDictionaries, targetLocale, defaultLocale);

    Variables variables = new(payload.Variables);
    IReadOnlyDictionary<string, string> variableDictionary = variables.AsDictionary();

    List<Message> messages = new(capacity: allRecipients.To.Count);
    foreach (Recipient recipient in allRecipients.To)
    {
      MessageId id = MessageId.NewId(command.TenantId);

      Dictionaries dictionaries = (payload.IgnoreUserLocale || recipient.User?.Locale == null)
        ? defaultDictionaries : new(allDictionaries, recipient.User.Locale, defaultLocale);

      Subject subject = new(dictionaries.Translate(new Identifier(template.Subject.Value)));
      Locale? locale = dictionaries.Target?.Locale ?? dictionaries.Default?.Locale;
      Content body = await _sender.Send(new CompileTemplateCommand(id, template, dictionaries, locale, recipient.User, variables), cancellationToken);
      IReadOnlyCollection<Recipient> recipients = [recipient, .. allRecipients.CC, .. allRecipients.Bcc];

      Message message = new(subject, body, recipients, sender, template, ignoreUserLocale, locale, variableDictionary, payload.IsDemo, actorId, id);
      messages.Add(message);

      await _sender.Send(new SendEmailCommand(actorId, message, sender), cancellationToken);
    }
    await _messageRepository.SaveAsync(messages, cancellationToken);

    return new SentMessages(messages.Select(x => x.EntityId.ToGuid()));
  }

  private async Task<Recipients> ResolveRecipientsAsync(TenantId? tenantId, SendMessagePayload payload, SenderType senderType, CancellationToken cancellationToken)
  {
    List<Recipient> recipients = new(capacity: payload.Recipients.Count);

    HashSet<UserId> userIds = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        UserId userId = new(tenantId, new EntityId(recipient.UserId.Value));
        userIds.Add(userId);
      }
    }

    Dictionary<Guid, User> users = new(recipients.Capacity);
    if (userIds.Count > 0)
    {
      IEnumerable<User> allUsers = await _userRepository.LoadAsync(userIds, cancellationToken);
      foreach (User user in allUsers)
      {
        users[user.EntityId.ToGuid()] = user;
      }
    }

    List<Guid> missingUsers = new(recipients.Capacity);
    List<Guid> missingContacts = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        if (users.TryGetValue(recipient.UserId.Value, out User? user))
        {
          switch (senderType)
          {
            case SenderType.Email:
              if (user.Email == null)
              {
                missingContacts.Add(recipient.UserId.Value);
                continue;
              }
              break;
            case SenderType.Sms:
              if (user.Phone == null)
              {
                missingContacts.Add(recipient.UserId.Value);
                continue;
              }
              break;
            default:
              throw new SenderTypeNotSupportedException(senderType);
          }

          recipients.Add(new Recipient(user, recipient.Type));
        }
        else
        {
          missingUsers.Add(recipient.UserId.Value);
        }
      }
      else
      {
        recipients.Add(new Recipient(recipient.Type, recipient.Address, recipient.DisplayName, recipient.PhoneNumber));
      }
    }
    if (missingUsers.Count > 0)
    {
      throw new UsersNotFoundException(missingUsers, nameof(payload.Recipients));
    }
    else if (missingContacts.Count > 0)
    {
      throw new MissingRecipientContactsException(missingContacts, nameof(payload.Recipients));
    }

    return new Recipients(recipients);
  }

  private async Task<Sender> ResolveSenderAsync(TenantId? tenantId, SendMessagePayload payload, CancellationToken cancellationToken)
  {
    if (payload.SenderId.HasValue)
    {
      SenderId senderId = new(tenantId, new EntityId(payload.SenderId.Value));
      return await _senderRepository.LoadAsync(senderId, cancellationToken)
        ?? throw new SenderNotFoundException(senderId.Value, nameof(payload.SenderId));
    }

    return await _senderRepository.LoadDefaultAsync(tenantId, cancellationToken)
      ?? throw new NoDefaultSenderException(tenantId);
  }

  private async Task<Template> ResolveTemplateAsync(TenantId? tenantId, SendMessagePayload payload, SenderType senderType, CancellationToken cancellationToken)
  {
    Template? template = null;
    if (Guid.TryParse(payload.Template, out Guid id))
    {
      TemplateId templateId = new(tenantId, new EntityId(id));
      template = await _templateRepository.LoadAsync(templateId, cancellationToken);
    }

    if (template == null)
    {
      try
      {
        Identifier uniqueKey = new(payload.Template);
        template = await _templateRepository.LoadAsync(tenantId, uniqueKey, cancellationToken);
      }
      catch (ValidationException)
      {
      }
    }

    if (template == null)
    {
      throw new TemplateNotFoundException(payload.Template, nameof(payload.Template));
    }
    else if (senderType == SenderType.Sms && template.Content.Type != MediaTypeNames.Text.Plain)
    {
      throw new InvalidSmsMessageContentTypeException(template.Content.Type, nameof(payload.Template));
    }

    return template;
  }
}
