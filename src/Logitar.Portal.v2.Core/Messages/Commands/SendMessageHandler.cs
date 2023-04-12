using Logitar.Portal.v2.Contracts.Messages;
using Logitar.Portal.v2.Core.Dictionaries;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Senders;
using Logitar.Portal.v2.Core.Templates;
using Logitar.Portal.v2.Core.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Messages.Commands;

internal class SendMessageHandler : IRequestHandler<SendMessage, SentMessages>
{
  private readonly ICurrentActor _currentActor;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IMediator _mediator;
  private readonly IMessageRepository _messageRepository;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  public SendMessageHandler(ICurrentActor currentActor,
    IDictionaryRepository dictionaryRepository,
    IMediator mediator,
    IMessageRepository messageRepository,
    IRealmRepository realmRepository,
    ISenderRepository senderRepository,
    ITemplateRepository templateRepository,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _dictionaryRepository = dictionaryRepository;
    _mediator = mediator;
    _messageRepository = messageRepository;
    _realmRepository = realmRepository;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
    _userRepository = userRepository;
  }

  public async Task<SentMessages> Handle(SendMessage request, CancellationToken cancellationToken)
  {
    SendMessageInput input = request.Input;
    // TODO(fpion): validate input

    RealmAggregate realm = await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));

    Recipients allRecipients = await _mediator.Send(new ResolveRecipients(realm, input.Recipients, nameof(input.Recipients)), cancellationToken);
    SenderAggregate sender = await _mediator.Send(new ResolveSender(realm, input.SenderId, nameof(input.SenderId)), cancellationToken);
    TemplateAggregate template = await _mediator.Send(new ResolveTemplate(realm, input.Template, nameof(input.Template)), cancellationToken);

    Dictionary<CultureInfo, DictionaryAggregate> allDictionaries = (await _dictionaryRepository.LoadAsync(realm, cancellationToken))
      .ToDictionary(x => x.Locale, x => x);
    DictionaryAggregate? defaultDictionary = null;
    if (realm.DefaultLocale != null)
    {
      _ = allDictionaries.TryGetValue(realm.DefaultLocale, out defaultDictionary);
    }

    Dictionary<string, string>? variables = input.Variables?.ToDictionary();

    CultureInfo? locale = input.Locale?.GetCultureInfo(nameof(input.Locale));
    Dictionaries? dictionaries = input.IgnoreUserLocale
      ? GetDictionaries(locale, defaultDictionary, allDictionaries)
      : null;

    List<MessageAggregate> messages = new(capacity: allRecipients.To.Count());
    int index = 0;
    foreach (Recipient recipient in allRecipients.To)
    {
      CultureInfo? userLocale = recipient.UserLocale == null ? null : CultureInfo.GetCultureInfo(recipient.UserLocale);
      Dictionaries? userDictionaries = (!input.IgnoreUserLocale && recipient.UserLocale != null)
        ? GetDictionaries(userLocale, defaultDictionary, allDictionaries)
        : null;

      string subject = (userDictionaries ?? dictionaries)?.GetEntry(template.Subject) ?? template.Subject;

      CompiledTemplate compiledTemplate = await _mediator.Send(new CompileTemplate(template,
        userDictionaries ?? dictionaries, recipient.User, variables), cancellationToken); // TODO(fpion): handler
      string body = compiledTemplate.Value;

      IEnumerable<Recipient> recipients = new[] { recipient }.Concat(allRecipients.CC).Concat(allRecipients.Bcc);

      MessageAggregate message = new(_currentActor.Id, realm, sender, template, subject, body,
        recipients, input.IgnoreUserLocale, locale, variables);

      messages.Add(message);

      index++;
    }

    await _messageRepository.SaveAsync(messages, cancellationToken);

    // TODO(fpion): send messages

    return new SentMessages
    {
      Error = messages.Where(x => x.HasErrors).Select(x => x.Id.ToGuid()),
      Success = messages.Where(x => x.Succeeded).Select(x => x.Id.ToGuid()),
      Unsent = messages.Where(x => !x.HasErrors && !x.Succeeded).Select(x => x.Id.ToGuid())
    };
  }

  private static Dictionaries GetDictionaries(CultureInfo? locale,
    DictionaryAggregate? defaultDictionary,
    Dictionary<CultureInfo, DictionaryAggregate> dictionaries)
  {
    DictionaryAggregate? preferredDictionary = null;
    DictionaryAggregate? fallbackDictionary = null;

    if (locale != null)
    {
      dictionaries.TryGetValue(locale, out preferredDictionary);

      if (locale.Parent != null)
      {
        dictionaries.TryGetValue(locale.Parent, out fallbackDictionary);
      }
    }

    return new Dictionaries(defaultDictionary, fallbackDictionary, preferredDictionary);
  }
}
