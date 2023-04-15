using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Templates;
using Logitar.Portal.Core.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Messages.Commands;

internal class SendDemoMessageHandler : IRequestHandler<SendDemoMessage, Message>
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

  public SendDemoMessageHandler(IApplicationContext applicationContext,
    IDictionaryRepository dictionaryRepository,
    IMediator mediator,
    IMessageQuerier messageQuerier,
    IMessageRepository messageRepository,
    IRealmRepository realmRepository,
    ISenderRepository senderRepository,
    ITemplateRepository templateRepository,
    IUserRepository userRepository)
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

  public async Task<Message> Handle(SendDemoMessage request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(_applicationContext.ActorId, cancellationToken)
      ?? throw new InvalidOperationException($"The user '{_applicationContext.ActorId}' could not be found.");
    Recipient[] recipients = new[] { Recipient.From(new RecipientInput(), user) };

    SendDemoMessageInput input = request.Input;

    TemplateAggregate template = await _templateRepository.LoadAsync(input.TemplateId, cancellationToken)
      ?? throw new AggregateNotFoundException<TemplateAggregate>(input.TemplateId, nameof(input.TemplateId));
    RealmAggregate? realm = await _realmRepository.LoadAsync(template, cancellationToken);
    SenderAggregate sender = await _senderRepository.LoadDefaultAsync(realm, cancellationToken)
      ?? throw new DefaultSenderRequiredException(realm);

    Dictionary<CultureInfo, DictionaryAggregate> allDictionaries = (await _dictionaryRepository.LoadAsync(realm, cancellationToken))
      .ToDictionary(x => x.Locale, x => x);
    CultureInfo defaultLocale = realm?.DefaultLocale ?? _applicationContext.Configuration.DefaultLocale;
    _ = allDictionaries.TryGetValue(defaultLocale, out DictionaryAggregate? defaultDictionary);

    CultureInfo? locale = input.Locale?.GetCultureInfo(nameof(input.Locale));
    Dictionaries dictionaries = MessageHelper.GetDictionaries(locale ?? user.Locale, defaultDictionary, allDictionaries);

    string subject = dictionaries.GetEntry(template.Subject) ?? template.Subject;

    Dictionary<string, string>? variables = input.Variables?.ToDictionary();
    CompiledTemplate compiledTemplate = await _mediator.Send(new CompileTemplate(template,
      dictionaries, user, variables), cancellationToken);
    string body = compiledTemplate.Value;

    MessageAggregate message = new(user.Id, realm, sender, template, subject, body,
      recipients, ignoreUserLocale: locale != null, locale, variables, isDemo: true);

    await _mediator.Publish(new SendEmail(message, sender), cancellationToken);

    await _messageRepository.SaveAsync(message, cancellationToken);

    return await _messageQuerier.GetAsync(message, cancellationToken);
  }
}
