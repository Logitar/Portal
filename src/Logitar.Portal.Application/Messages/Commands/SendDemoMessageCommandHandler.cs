using Logitar.EventSourcing;
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

internal class SendDemoMessageCommandHandler : IRequestHandler<SendDemoMessageCommand, Message>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IMediator _mediator;
  private readonly IMessageQuerier _messageQuerier;
  private readonly IMessageRepository _messageRepository;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  public SendDemoMessageCommandHandler(IApplicationContext applicationContext, IDictionaryRepository dictionaryRepository,
    IMediator mediator, IMessageQuerier messageQuerier, IMessageRepository messageRepository, IRealmRepository realmRepository,
    ISenderRepository senderRepository, ITemplateRepository templateRepository, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryRepository = dictionaryRepository;
    _mediator = mediator;
    _messageQuerier = messageQuerier;
    _messageRepository = messageRepository;
    _realmRepository = realmRepository;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
    _userRepository = userRepository;
  }

  public async Task<Message> Handle(SendDemoMessageCommand command, CancellationToken cancellationToken)
  {
    AggregateId userId = new(_applicationContext.ActorId.Value);
    UserAggregate user = await _userRepository.LoadAsync(userId, version: null, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(userId, nameof(_applicationContext.ActorId));

    SendDemoMessagePayload payload = command.Payload;
    bool ignoreUserLocale = payload.Locale != null;
    ReadOnlyLocale? locale = payload.Locale?.GetLocale(nameof(payload.Locale)) ?? user.Locale;

    TemplateAggregate template = await ResolveTemplateAsync(payload, cancellationToken);
    RealmAggregate? realm = await ResolveRealmAsync(template, cancellationToken);
    SenderAggregate sender = await ResolveSenderAsync(payload, realm, cancellationToken);
    Recipients recipients = ResolveRecipients(realm, user);

    Dictionary<ReadOnlyLocale, DictionaryAggregate> allDictionaries = (await _dictionaryRepository.LoadAsync(realm, cancellationToken))
      .ToDictionary(x => x.Locale, x => x);
    Dictionaries dictionaries = ResolveDictionaries(allDictionaries, locale, realm);

    Variables variables = new(payload.Variables.Select(variable => new KeyValuePair<string, string>(variable.Key, variable.Value)));

    CompiledTemplate compiledTemplate = await _mediator.Send(new CompileTemplateCommand(template, dictionaries, user, variables), cancellationToken);

    string subject = dictionaries.Translate(template.Subject);
    string body = compiledTemplate.Value;

    MessageAggregate message = new(subject, body, recipients, sender, template, realm,
      ignoreUserLocale, locale, variables, isDemo: true, _applicationContext.ActorId);

    await _mediator.Publish(new SendEmailCommand(message, sender), cancellationToken);

    await _messageRepository.SaveAsync(message, cancellationToken);

    return await _messageQuerier.ReadAsync(message, cancellationToken);
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

  private Recipients ResolveRecipients(RealmAggregate? realm, UserAggregate user)
  {
    ReadOnlyRecipient recipient;
    if (realm?.Id.Value == user.TenantId)
    {
      recipient = ReadOnlyRecipient.From(user);
    }
    else if (user.Email == null)
    {
      string[] recipients = new[] { _applicationContext.ActorId.ToGuid().ToString() };
      throw new MissingRecipientAddressesException(recipients, nameof(_applicationContext.ActorId));
    }
    else
    {
      recipient = new(user.Email.Address, user.FullName, userId: user.Id);
    }

    return new Recipients(new[] { recipient });
  }

  private async Task<RealmAggregate?> ResolveRealmAsync(TemplateAggregate template, CancellationToken cancellationToken)
  {
    return await _realmRepository.LoadAsync(template, cancellationToken);
  }

  private async Task<SenderAggregate> ResolveSenderAsync(SendDemoMessagePayload payload, RealmAggregate? realm, CancellationToken cancellationToken)
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
      ?? throw new RealmHasNoDefaultSenderException(realm, nameof(payload.TemplateId));
  }

  private async Task<TemplateAggregate> ResolveTemplateAsync(SendDemoMessagePayload payload, CancellationToken cancellationToken)
  {
    return await _templateRepository.LoadAsync(payload.TemplateId, cancellationToken)
      ?? throw new AggregateNotFoundException<TemplateAggregate>(payload.TemplateId, nameof(payload.TemplateId));
  }
}
