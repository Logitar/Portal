using FluentValidation;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Application.Messages.Commands
{
  internal class SendDemoMessageCommandHandler : IRequestHandler<SendDemoMessageCommand, MessageModel>
  {
    private readonly IMessageHandlerFactory _messageHandlerFactory;
    private readonly IMessageQuerier _messageQuerier;
    private readonly IValidator<Message> _messageValidator;
    private readonly IRepository _repository;
    private readonly ITemplateCompiler _templateCompiler;
    private readonly IUserContext _userContext;

    public SendDemoMessageCommandHandler(IMessageHandlerFactory messageHandlerFactory,
      IMessageQuerier messageQuerier,
      IValidator<Message> messageValidator,
      IRepository repository,
      ITemplateCompiler templateCompiler,
      IUserContext userContext)
    {
      _messageHandlerFactory = messageHandlerFactory;
      _messageQuerier = messageQuerier;
      _messageValidator = messageValidator;
      _repository = repository;
      _templateCompiler = templateCompiler;
      _userContext = userContext;
    }

    public async Task<MessageModel> Handle(SendDemoMessageCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(_userContext.UserId, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={_userContext.UserId}' could not be found.");
      if (user.Email == null)
      {
        throw new UserEmailRequiredException(user);
      }
      Recipient[] recipients = new Recipient[] { new(user) };

      SendDemoMessagePayload payload = request.Payload;

      Template template = await _repository.LoadAsync<Template>(payload.TemplateId, cancellationToken)
        ?? throw new EntityNotFoundException<Template>(payload.TemplateId, nameof(payload.TemplateId));

      Realm? realm = template.RealmId.HasValue
        ? (await _repository.LoadAsync<Realm>(template.RealmId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm 'Id={template.RealmId}' could not be found."))
        : null;

      Sender sender = await _repository.LoadDefaultSenderAsync(realm, cancellationToken)
        ?? throw new DefaultSenderRequiredException(realm);

      Dictionary<CultureInfo, Dictionary> allDictionaries = (await _repository.LoadDictionariesByRealmAsync(realm, cancellationToken))
        .ToDictionary(x => x.Locale, x => x);
      Dictionary? defaultDictionary = null;
      if (realm?.DefaultLocale != null)
      {
        allDictionaries.TryGetValue(realm.DefaultLocale, out defaultDictionary);
      }
      Dictionaries dictionaries = MessageHelper.GetDictionaries(payload.Locale ?? user.Locale, defaultDictionary, allDictionaries);

      Dictionary<string, string?>? variables = payload.Variables == null ? null : MessageHelper.GetVariables(payload.Variables);

      IMessageHandler handler = _messageHandlerFactory.GetHandler(sender);

      string subject = dictionaries.GetEntry(template.Subject) ?? template.Subject;
      string body = _templateCompiler.Compile(template, dictionaries, user, variables);

      Message message = new(_userContext.ActorId, subject, body, recipients, sender, template, realm, ignoreUserLocale: payload.Locale != null, payload.Locale ?? user.Locale, variables, isDemo: true);
      _messageValidator.ValidateAndThrow(message);

      try
      {
        SendMessageResult result = await handler.SendAsync(message, cancellationToken);
        message.Succeed(_userContext.ActorId, result);
      }
      catch (ErrorException exception)
      {
        message.Fail(_userContext.ActorId, exception.Error);
      }
      catch (Exception exception)
      {
        message.Fail(_userContext.ActorId, new Error(exception));
      }

      await _repository.SaveAsync(message, cancellationToken);

      return await _messageQuerier.GetAsync(message.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The message 'Id={message.Id}' could not be found.");
    }
  }
}
