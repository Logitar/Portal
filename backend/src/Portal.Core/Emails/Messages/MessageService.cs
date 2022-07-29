using FluentValidation;
using Portal.Core.Emails.Messages.Payloads;
using Portal.Core.Emails.Senders;
using Portal.Core.Emails.Templates;
using Portal.Core.Realms;
using Portal.Core.Users;

namespace Portal.Core.Emails.Messages
{
  internal class MessageService : IMessageService
  {
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<Message> _repository;
    private readonly ISenderQuerier _senderQuerier;
    private readonly ITemplateCompiler _templateCompiler;
    private readonly ITemplateQuerier _templateQuerier;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IValidator<SendMessagePayload> _validator;

    public MessageService(
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
      _realmQuerier = realmQuerier;
      _repository = repository;
      _senderQuerier = senderQuerier;
      _templateCompiler = templateCompiler;
      _templateQuerier = templateQuerier;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _validator = validator;
    }

    public async Task SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(payload);

      _validator.ValidateAndThrow(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = (Guid.TryParse(payload.Realm, out Guid guid)
          ? await _realmQuerier.GetAsync(guid, readOnly: true, cancellationToken)
          : await _realmQuerier.GetAsync(alias: payload.Realm, readOnly: true, cancellationToken)
        ) ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      Template template = (Guid.TryParse(payload.Template, out Guid templateId)
        ? await _templateQuerier.GetAsync(templateId, readOnly: true, cancellationToken)
        : await _templateQuerier.GetAsync(key: payload.Template, realm, readOnly: true, cancellationToken)
      ) ?? throw new EntityNotFoundException<Template>(payload.Template, nameof(payload.Template));
      if (realm?.Sid != template.RealmSid)
      {
        throw new NotImplementedException(); // TODO(fpion): implement
      }

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
        throw new NotImplementedException(); // TODO(fpion): implement
      }

      IEnumerable<Guid> userIds = payload.Recipients.Where(x => x.UserId.HasValue).Select(x => x.UserId!.Value);
      Dictionary<Guid, User> users = (await _userQuerier.GetAsync(userIds, readOnly: true, cancellationToken))
        .ToDictionary(x => x.Id, x => x);

      var missingUsers = new List<Guid>(capacity: payload.Recipients.Count());
      var to = new List<Recipient>(capacity: missingUsers.Count);
      var cc = new List<Recipient>(capacity: missingUsers.Count);
      var bcc = new List<Recipient>(capacity: missingUsers.Count);
      foreach (RecipientPayload recipientPayload in payload.Recipients)
      {
        User? user = null;
        if (recipientPayload.UserId.HasValue && (!users.TryGetValue(recipientPayload.UserId.Value, out user)))
        {
          missingUsers.Add(recipientPayload.UserId.Value);

          continue;
        }

        if (user != null && realm?.Sid != user.RealmSid)
        {
          throw new NotImplementedException(); // TODO(fpion): implement
        }

        Recipient recipient = user == null
          ? new(recipientPayload.Address!, recipientPayload.DisplayName, recipientPayload.Type)
          : new(user);

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

      Dictionary<string, string?>? variables = payload.Variables
        ?.GroupBy(x => x.Key)
        .ToDictionary(x => x.Key, x => x.FirstOrDefault(y => y.Value != null)?.Value);

      var messages = new List<Message>(capacity: to.Count);

      foreach (Recipient recipient in to)
      {
        string body = _templateCompiler.Compile(template, recipient.User, variables);

        IEnumerable<Recipient> recipients = new[] { recipient }.Concat(cc).Concat(bcc);

        var message = new Message(body, recipients, sender, template, _userContext.ActorId, realm, variables);

        // TODO(fpion): send message

        messages.Add(message);
      }

      await _repository.SaveAsync(messages, cancellationToken);

      // TODO(fpion): response; differentiate success/failures
    }
  }
}
