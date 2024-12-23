using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
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
    LocaleUnit? defaultLocale = command.DefaultLocale;

    SendMessagePayload payload = command.Payload;
    Sender sender = await ResolveSenderAsync(tenantId, payload, cancellationToken);
    new SendMessageValidator(sender.Type).ValidateAndThrow(payload);

    Recipients allRecipients = await ResolveRecipientsAsync(payload, sender.Type, cancellationToken);
    TemplateAggregate template = await ResolveTemplateAsync(tenantId, payload, sender.Type, cancellationToken);

    Dictionary<LocaleUnit, Dictionary> allDictionaries = (await _dictionaryRepository.LoadAsync(tenantId, cancellationToken))
      .ToDictionary(x => x.Locale, x => x);
    bool ignoreUserLocale = payload.IgnoreUserLocale;
    LocaleUnit? targetLocale = LocaleUnit.TryCreate(payload.Locale);
    Dictionaries defaultDictionaries = new(allDictionaries, targetLocale, defaultLocale);

    Variables variables = new(payload.Variables);
    IReadOnlyDictionary<string, string> variableDictionary = variables.AsDictionary();

    List<MessageAggregate> messages = new(capacity: allRecipients.To.Count);
    foreach (RecipientUnit recipient in allRecipients.To)
    {
      MessageId id = MessageId.NewId();

      Dictionaries dictionaries = (payload.IgnoreUserLocale || recipient.User?.Locale == null)
        ? defaultDictionaries : new(allDictionaries, recipient.User.Locale, defaultLocale);

      SubjectUnit subject = new(dictionaries.Translate(template.Subject.Value));
      LocaleUnit? locale = dictionaries.Target?.Locale ?? dictionaries.Default?.Locale;
      ContentUnit body = await _sender.Send(new CompileTemplateCommand(id, template, dictionaries, locale, recipient.User, variables), cancellationToken);
      IReadOnlyCollection<RecipientUnit> recipients = [recipient, .. allRecipients.CC, .. allRecipients.Bcc];

      MessageAggregate message = new(subject, body, recipients, sender, template, ignoreUserLocale, locale, variableDictionary, payload.IsDemo, tenantId, actorId, id);
      messages.Add(message);

      await _sender.Send(new SendEmailCommand(actorId, message, sender), cancellationToken);
    }
    await _messageRepository.SaveAsync(messages, cancellationToken);

    return new SentMessages(messages.Select(x => x.Id.ToGuid()));
  }

  private async Task<Recipients> ResolveRecipientsAsync(SendMessagePayload payload, SenderType senderType, CancellationToken cancellationToken)
  {
    List<RecipientUnit> recipients = new(capacity: payload.Recipients.Count);

    HashSet<UserId> userIds = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        userIds.Add(new UserId(recipient.UserId.Value));
      }
    }

    Dictionary<Guid, UserAggregate> users = new(recipients.Capacity);
    if (userIds.Count > 0)
    {
      IEnumerable<UserAggregate> allUsers = await _userRepository.LoadAsync(userIds, cancellationToken);
      foreach (UserAggregate user in allUsers)
      {
        users[user.Id.ToGuid()] = user;
      }
    }

    List<Guid> missingUsers = new(recipients.Capacity);
    List<Guid> missingContacts = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        if (users.TryGetValue(recipient.UserId.Value, out UserAggregate? user))
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

          recipients.Add(new RecipientUnit(user, recipient.Type));
        }
        else
        {
          missingUsers.Add(recipient.UserId.Value);
        }
      }
      else
      {
        recipients.Add(new RecipientUnit(recipient.Type, recipient.Address, recipient.DisplayName, recipient.PhoneNumber));
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
      return await _senderRepository.LoadAsync(payload.SenderId.Value, cancellationToken)
        ?? throw new SenderNotFoundException(payload.SenderId.Value, nameof(payload.SenderId));
    }

    return await _senderRepository.LoadDefaultAsync(tenantId, cancellationToken)
      ?? throw new NoDefaultSenderException(tenantId);
  }

  private async Task<TemplateAggregate> ResolveTemplateAsync(TenantId? tenantId, SendMessagePayload payload, SenderType senderType, CancellationToken cancellationToken)
  {
    TemplateAggregate? template = null;
    if (Guid.TryParse(payload.Template, out Guid id))
    {
      template = await _templateRepository.LoadAsync(id, cancellationToken);
    }

    if (template == null)
    {
      try
      {
        UniqueKeyUnit uniqueKey = new(payload.Template);
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
