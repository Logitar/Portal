using AutoMapper;
using FluentValidation;
using Portal.Core.Emails.Messages.Models;
using Portal.Core.Emails.Messages.Payloads;
using Portal.Core.Emails.Senders;
using Portal.Core.Emails.Templates;
using Portal.Core.Realms;
using Portal.Core.Users;

namespace Portal.Core.Emails.Messages
{
  internal class MessageService : IMessageService
  {
    private readonly IMessageHandlerFactory _handlerFactory;
    private readonly IMapper _mapper;
    private readonly IMessageQuerier _querier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<Message> _repository;
    private readonly ISenderQuerier _senderQuerier;
    private readonly ITemplateCompiler _templateCompiler;
    private readonly ITemplateQuerier _templateQuerier;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IValidator<SendMessagePayload> _validator;

    public MessageService(
      IMessageHandlerFactory handlerFactory,
      IMapper mapper,
      IMessageQuerier querier,
      IRealmQuerier realmQuerier,
      IRepository<Message> repository,
      ISenderQuerier senderQuerier,
      ITemplateCompiler templateCompiler,
      ITemplateQuerier templateQuerier,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IValidator<SendMessagePayload> validator
    )
    {
      _handlerFactory = handlerFactory;
      _mapper = mapper;
      _querier = querier;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _senderQuerier = senderQuerier;
      _templateCompiler = templateCompiler;
      _templateQuerier = templateQuerier;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _validator = validator;
    }

    public async Task<MessageModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      Message? message = await _querier.GetAsync(id, readOnly: true, cancellationToken);

      return _mapper.Map<MessageModel>(message);
    }

    public async Task<ListModel<MessageSummary>> GetAsync(bool? hasErrors, Guid? realmId, string? search, bool? succeeded, Guid? templateId,
      MessageSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<Message> messages = await _querier.GetPagedAsync(hasErrors, realmId, search, succeeded, templateId,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return ListModel<MessageSummary>.From(messages, _mapper);
    }

    public async Task<SentMessagesModel> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      _validator.ValidateAndThrow(payload);

      /* ---------------------------------------- Realm ----------------------------------------- */
      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = (Guid.TryParse(payload.Realm, out Guid guid)
          ? await _realmQuerier.GetAsync(guid, readOnly: true, cancellationToken)
          : await _realmQuerier.GetAsync(alias: payload.Realm, readOnly: true, cancellationToken)
        ) ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      /* -------------------------------------- Template ---------------------------------------- */
      Template template = (Guid.TryParse(payload.Template, out Guid templateId)
        ? await _templateQuerier.GetAsync(templateId, readOnly: true, cancellationToken)
        : await _templateQuerier.GetAsync(key: payload.Template, realm, readOnly: true, cancellationToken)
      ) ?? throw new EntityNotFoundException<Template>(payload.Template, nameof(payload.Template));
      if (realm?.Sid != template.RealmSid)
      {
        throw new TemplateNotInRealmException(template, realm, nameof(payload.Template));
      }

      /* --------------------------------------- Sender ----------------------------------------- */
      Sender sender;
      if (payload.SenderId.HasValue)
      {
        sender = await _senderQuerier.GetAsync(payload.SenderId.Value, readOnly: true, cancellationToken)
          ?? throw new EntityNotFoundException<Sender>(payload.SenderId.Value, nameof(payload.SenderId));
      }
      else
      {
        sender = await _senderQuerier.GetDefaultAsync(realm, readOnly: true, cancellationToken)
          ?? throw new DefaultSenderRequiredException(realm);
      }
      if (realm?.Sid != sender.RealmSid)
      {
        throw new SenderNotInRealmException(sender, realm, nameof(payload.SenderId));
      }
      IMessageHandler handler = _handlerFactory.GetHandler(sender);

      /* ------------------------------------- Recipients --------------------------------------- */
      var userIds = new List<Guid>(capacity: payload.Recipients.Count());
      var usernames = new List<string>(userIds.Capacity);
      foreach (RecipientPayload recipient in payload.Recipients)
      {
        if (recipient.User != null)
        {
          if (Guid.TryParse(recipient.User, out Guid userId))
          {
            userIds.Add(userId);
          }
          else
          {
            usernames.Add(recipient.User);
          }
        }
      }
      
      Dictionary<Guid, User> usersById = (await _userQuerier.GetAsync(userIds, readOnly: true, cancellationToken))
        .ToDictionary(x => x.Id, x => x);
      Dictionary<string, User> usersByUsername = (await _userQuerier.GetAsync(usernames, realm, readOnly: true, cancellationToken))
        .ToDictionary(x => x.UsernameNormalized, x => x);

      var missingUsers = new List<string>(userIds.Capacity);
      var notInRealm = new List<Guid>(userIds.Capacity);
      var to = new List<Recipient>(userIds.Capacity);
      var cc = new List<Recipient>(userIds.Capacity);
      var bcc = new List<Recipient>(userIds.Capacity);
      foreach (RecipientPayload recipientPayload in payload.Recipients)
      {
        User? user = null;
        if (recipientPayload.User != null)
        {
          if (Guid.TryParse(recipientPayload.User, out Guid userId))
          {
            usersById.TryGetValue(userId, out user);
          }
          else
          {
            usersByUsername.TryGetValue(recipientPayload.User.ToUpper(), out user);
          }

          if (user == null)
          {
            missingUsers.Add(recipientPayload.User);
            continue;
          }
          else if (realm?.Sid != user.RealmSid)
          {
            notInRealm.Add(user.Id);
            continue;
          }
        }

        Recipient recipient = user == null
          ? new(recipientPayload.Address!, recipientPayload.DisplayName, recipientPayload.Type)
          : Recipient.FromUser(user, recipientPayload.Type);

        switch (recipientPayload.Type)
        {
          case RecipientType.Bcc:
            bcc.Add(recipient);
            break;
          case RecipientType.CC:
            cc.Add(recipient);
            break;
          case RecipientType.To:
            to.Add(recipient);
            break;
          default:
            throw new NotSupportedException($"The recipient type '{recipientPayload.Type}' is not supported.");
        }
      }
      if (missingUsers.Any())
      {
        throw new UsersNotFoundException(missingUsers, nameof(payload.Recipients));
      }
      if (notInRealm.Any())
      {
        throw new UsersNotInRealmException(notInRealm, realm, nameof(payload.Recipients));
      }

      /* -------------------------------------- Messages ---------------------------------------- */
      Dictionary<string, string?>? variables = payload.Variables
        ?.GroupBy(x => x.Key)
        .ToDictionary(x => x.Key, x => x.FirstOrDefault(y => y.Value != null)?.Value);

      var messages = new List<Message>(capacity: to.Count);

      foreach (Recipient recipient in to)
      {
        string body = _templateCompiler.Compile(template, recipient.User, variables);

        IEnumerable<Recipient> recipients = new[] { recipient }.Concat(cc).Concat(bcc);

        var message = new Message(body, recipients, sender, template, _userContext.ActorId, realm, variables);

        try
        {
          SendMessageResult result = await handler.SendAsync(message, cancellationToken);
          message.Succeed(result, _userContext.Id);
        }
        catch (ErrorException exception)
        {
          message.Fail(exception.Error, _userContext.Id);
        }
        catch (Exception exception)
        {
          message.Fail(new Error(exception), _userContext.Id);
        }

        messages.Add(message);
      }

      await _repository.SaveAsync(messages, cancellationToken);

      return new SentMessagesModel(messages);
    }
  }
}
