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

internal class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SentMessages>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IMediator _mediator;
  private readonly IMessageRepository _messageRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  public SendMessageCommandHandler(IApplicationContext applicationContext, IDictionaryRepository dictionaryRepository, IMediator mediator,
    IMessageRepository messageRepository, ISenderRepository senderRepository, ITemplateRepository templateRepository, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryRepository = dictionaryRepository;
    _mediator = mediator;
    _messageRepository = messageRepository;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
    _userRepository = userRepository;
  }

  public async Task<SentMessages> Handle(SendMessageCommand command, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = command.Payload;
    new SendMessageValidator().ValidateAndThrow(payload);

    ActorId actorId = _applicationContext.ActorId;
    TenantId? tenantId = _applicationContext.TenantId;

    Recipients allRecipients = await ResolveRecipientsAsync(payload, cancellationToken);
    SenderAggregate sender = await ResolveSenderAsync(tenantId, payload, cancellationToken);
    TemplateAggregate template = await ResolveTemplateAsync(tenantId, payload, cancellationToken);

    Dictionary<LocaleUnit, DictionaryAggregate> allDictionaries = (await _dictionaryRepository.LoadAsync(tenantId, cancellationToken))
      .ToDictionary(x => x.Locale, x => x);
    bool ignoreUserLocale = payload.IgnoreUserLocale;
    LocaleUnit? targetLocale = LocaleUnit.TryCreate(payload.Locale);
    LocaleUnit? defaultLocale = LocaleUnit.TryCreate((_applicationContext.Realm?.DefaultLocale ?? _applicationContext.Configuration.DefaultLocale)?.Code);
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
      ContentUnit body = await _mediator.Send(new CompileTemplateCommand(id, template, dictionaries, locale, recipient.User, variables), cancellationToken);
      IReadOnlyCollection<RecipientUnit> recipients = [recipient, .. allRecipients.CC, .. allRecipients.Bcc];

      MessageAggregate message = new(subject, body, recipients, sender, template, ignoreUserLocale, locale, variableDictionary, payload.IsDemo, tenantId, actorId, id);
      messages.Add(message);

      await _mediator.Publish(new SendEmailCommand(actorId, message, sender), cancellationToken);
    }
    await _messageRepository.SaveAsync(messages, cancellationToken);

    return new SentMessages(messages.Select(x => x.Id.ToGuid()));
  }

  private async Task<Recipients> ResolveRecipientsAsync(SendMessagePayload payload, CancellationToken cancellationToken)
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
    List<Guid> missingEmails = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        if (users.TryGetValue(recipient.UserId.Value, out UserAggregate? user))
        {
          if (user.Email == null)
          {
            missingEmails.Add(recipient.UserId.Value);
          }
          else
          {
            recipients.Add(new RecipientUnit(user, recipient.Type));
          }
        }
        else
        {
          missingUsers.Add(recipient.UserId.Value);
        }
      }
      else
      {
        recipients.Add(new RecipientUnit(recipient.Address ?? string.Empty, recipient.DisplayName, recipient.Type));
      }
    }
    if (missingUsers.Count > 0)
    {
      throw new UsersNotFoundException(missingUsers, nameof(payload.Recipients));
    }
    else if (missingEmails.Count > 0)
    {
      throw new MissingRecipientAddressesException(missingEmails, nameof(payload.Recipients));
    }

    return new Recipients(recipients);
  }

  private async Task<SenderAggregate> ResolveSenderAsync(TenantId? tenantId, SendMessagePayload payload, CancellationToken cancellationToken)
  {
    if (payload.SenderId.HasValue)
    {
      return await _senderRepository.LoadAsync(payload.SenderId.Value, cancellationToken)
        ?? throw new SenderNotFoundException(payload.SenderId.Value, nameof(payload.SenderId));
    }

    return await _senderRepository.LoadDefaultAsync(tenantId, cancellationToken)
      ?? throw new NoDefaultSenderException(tenantId);
  }

  private async Task<TemplateAggregate> ResolveTemplateAsync(TenantId? tenantId, SendMessagePayload payload, CancellationToken cancellationToken)
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

    return template ?? throw new TemplateNotFoundException(payload.Template, nameof(payload.Template));
  }
}
