using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

internal class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SentMessages>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IMediator _mediator;
  private readonly IMessageRepository _messageRepository;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  public SendMessageCommandHandler(IApplicationContext applicationContext, IDictionaryRepository dictionaryRepository,
    IMediator mediator, IMessageRepository messageRepository, IRealmRepository realmRepository,
    ISenderRepository senderRepository, ITemplateRepository templateRepository, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryRepository = dictionaryRepository;
    _mediator = mediator;
    _messageRepository = messageRepository;
    _realmRepository = realmRepository;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
    _userRepository = userRepository;
  }

  public async Task<SentMessages> Handle(SendMessageCommand command, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = command.Payload;
    ReadOnlyLocale? sharedLocale = payload.Locale?.GetLocale(nameof(payload.Locale));

    RealmAggregate? realm = await ResolveRealmAsync(command, cancellationToken);
    SenderAggregate? sender = await ResolveSenderAsync(payload, realm, cancellationToken);
    TemplateAggregate? template = await ResolveTemplateAsync(command, realm, cancellationToken);
    Recipients allRecipients = await ResolveRecipientsAsync(payload, realm, cancellationToken);

    Dictionary<ReadOnlyLocale, DictionaryAggregate> allDictionaries = (await _dictionaryRepository.LoadAsync(realm, cancellationToken))
      .ToDictionary(x => x.Locale, x => x);
    Dictionaries sharedDictionaries = ResolveDictionaries(allDictionaries, sharedLocale, realm);

    Variables variables = new(payload.Variables.Select(variable => new KeyValuePair<string, string>(variable.Key, variable.Value)));

    List<MessageAggregate> messages = new(capacity: allRecipients.To.Count);

    foreach (ReadOnlyRecipient recipient in allRecipients.To)
    {
      ReadOnlyLocale? locale = sharedLocale;
      Dictionaries dictionaries = sharedDictionaries;
      if (!payload.IgnoreUserLocale && recipient.User != null && recipient.User.Locale != sharedLocale)
      {
        locale = recipient.User.Locale;
        dictionaries = ResolveDictionaries(allDictionaries, locale, realm);
      }

      CompiledTemplate compiledTemplate = await _mediator.Send(new CompileTemplateCommand(template, dictionaries, recipient.User, variables), cancellationToken);

      string subject = dictionaries.Translate(template.Subject);
      string body = compiledTemplate.Value;

      Recipients recipients = new(new[] { recipient }.Concat(allRecipients.CC).Concat(allRecipients.Bcc));

      MessageAggregate message = new(subject, body, recipients, sender, template, realm,
        payload.IgnoreUserLocale, locale, variables, command.IsDemo, _applicationContext.ActorId);
      messages.Add(message);

      await _mediator.Publish(new SendEmailCommand(message, sender), cancellationToken);
    }

    await _messageRepository.SaveAsync(messages, cancellationToken);

    return new SentMessages(messages.Select(message => message.Id.ToGuid()));
  }

  private Dictionaries ResolveDictionaries(Dictionary<ReadOnlyLocale, DictionaryAggregate> dictionaries, ReadOnlyLocale? locale, RealmAggregate? realm)
  {
    DictionaryAggregate? target = null;
    if (locale != null)
    {
      _ = dictionaries.TryGetValue(locale, out target);
    }

    DictionaryAggregate? fallback = null;
    if (locale?.Parent != null)
    {
      _ = dictionaries.TryGetValue(locale.Parent, out fallback);
    }

    ReadOnlyLocale defaultLocale = realm?.DefaultLocale ?? _applicationContext.Configuration.DefaultLocale;
    _ = dictionaries.TryGetValue(defaultLocale, out DictionaryAggregate? @default);

    return new Dictionaries(target, fallback, @default);
  }

  private async Task<RealmAggregate?> ResolveRealmAsync(SendMessageCommand command, CancellationToken cancellationToken)
  {
    if (command.Realm != null)
    {
      return command.Realm;
    }

    SendMessagePayload payload = command.Payload;

    if (string.IsNullOrWhiteSpace(payload.Realm))
    {
      return null;
    }

    return await _realmRepository.FindAsync(payload.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
  }

  private async Task<Recipients> ResolveRecipientsAsync(SendMessagePayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    bool uniqueEmail = realm?.RequireUniqueEmail ?? false;

    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(realm, cancellationToken);

    int capacity = users.Count();
    Dictionary<Guid, UserAggregate> usersById = new(capacity);
    Dictionary<string, UserAggregate> usersByUniqueName = new(capacity);
    foreach (UserAggregate user in users)
    {
      usersById[user.Id.ToGuid()] = user;
      usersByUniqueName[user.UniqueName.ToUpper()] = user;

      if (user.Email != null && uniqueEmail)
      {
        usersByUniqueName[user.Email.Address.ToUpper()] = user;
      }
    }

    List<ReadOnlyRecipient> recipients = new(capacity);
    List<string> missingUsers = new(capacity);
    List<string> missingEmails = new(capacity);

    int index = 0;
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (string.IsNullOrWhiteSpace(recipient.User))
      {
        if (string.IsNullOrWhiteSpace(recipient.Address))
        {
          missingEmails.Add($"{nameof(payload.Recipients)}[{index}].{nameof(recipient.Address)}");
        }
        else
        {
          recipients.Add(new ReadOnlyRecipient(recipient.Address, recipient.DisplayName, recipient.Type));
        }
      }
      else
      {
        string userId = recipient.User.Trim();
        string uniqueName = userId.ToUpper();

        _ = Guid.TryParse(userId, out Guid id)
          ? usersById.TryGetValue(id, out UserAggregate? user)
          : usersByUniqueName.TryGetValue(uniqueName, out user);
        if (user == null)
        {
          missingUsers.Add(recipient.User);
        }
        else if (user.Email == null)
        {
          missingEmails.Add($"{nameof(payload.Recipients)}[{index}].{nameof(recipient.User)}:{user.Id.ToGuid()}");
        }
        else
        {
          recipients.Add(ReadOnlyRecipient.From(user));
        }
      }

      index++;
    }

    if (missingEmails.Any())
    {
      throw new MissingRecipientAddressesException(missingEmails, nameof(payload.Recipients));
    }
    if (missingUsers.Any())
    {
      throw new UsersNotFoundException(missingUsers, nameof(payload.Recipients));
    }

    return new Recipients(recipients);
  }

  private async Task<SenderAggregate> ResolveSenderAsync(SendMessagePayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    if (payload.SenderId.HasValue)
    {
      SenderAggregate sender = await _senderRepository.LoadAsync(payload.SenderId.Value, cancellationToken)
        ?? throw new AggregateNotFoundException<SenderAggregate>(payload.SenderId.Value, nameof(payload.SenderId));

      if (sender.TenantId != realm?.Id.Value)
      {
        throw new SenderNotInRealmException(sender, realm, nameof(payload.SenderId));
      }

      return sender;
    }

    string? tenantId = realm?.Id.Value;

    return await _senderRepository.LoadDefaultAsync(tenantId, cancellationToken)
      ?? throw new RealmHasNoDefaultSenderException(realm, nameof(payload.Realm));
  }

  private async Task<TemplateAggregate> ResolveTemplateAsync(SendMessageCommand command, RealmAggregate? realm, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = command.Payload;
    string? tenantId = realm?.Id.Value;

    TemplateAggregate? template = command.Template ?? (Guid.TryParse(payload.Template, out Guid id)
      ? await _templateRepository.LoadAsync(id, cancellationToken)
      : await _templateRepository.LoadAsync(tenantId, payload.Template, cancellationToken));

    if (template == null)
    {
      throw new AggregateNotFoundException<TemplateAggregate>(payload.Template, nameof(payload.Template));
    }
    else if (template.TenantId != tenantId)
    {
      throw new TemplateNotInRealmException(template, realm, nameof(payload.Template));
    }

    return template;
  }
}
