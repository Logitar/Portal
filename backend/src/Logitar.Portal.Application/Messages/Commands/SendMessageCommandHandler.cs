using FluentValidation;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain;
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
  internal class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SentMessagesModel>
  {
    private readonly IMessageHandlerFactory _messageHandlerFactory;
    private readonly IValidator<Message> _messageValidator;
    private readonly IRepository _repository;
    private readonly ITemplateCompiler _templateCompiler;
    private readonly IUserContext _userContext;

    public SendMessageCommandHandler(IMessageHandlerFactory messageHandlerFactory,
      IValidator<Message> messageValidator,
      IRepository repository,
      ITemplateCompiler templateCompiler,
      IUserContext userContext)
    {
      _messageHandlerFactory = messageHandlerFactory;
      _messageValidator = messageValidator;
      _repository = repository;
      _templateCompiler = templateCompiler;
      _userContext = userContext;
    }

    public async Task<SentMessagesModel> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
      SendMessagePayload payload = request.Payload;

      Realm? realm = payload.Realm == null
        ? null
        : await ResolveRealmAsync(payload.Realm, nameof(payload.Realm), cancellationToken);

      Recipients allRecipients = await ResolveRecipientsAsync(payload.Recipients, realm, nameof(payload.Recipients), cancellationToken);
      Sender sender = await ResolveSenderAsync(payload.SenderId, realm, nameof(payload.SenderId), cancellationToken);
      Template template = await ResolveTemplateAsync(payload.Template, realm, nameof(payload.Template), cancellationToken);

      Dictionary<CultureInfo, Dictionary> allDictionaries = (await _repository.LoadDictionariesByRealmAsync(realm, cancellationToken))
        .ToDictionary(x => x.Locale, x => x);
      Dictionary? defaultDictionary = null;
      if (realm?.DefaultLocale != null)
      {
        allDictionaries.TryGetValue(realm.DefaultLocale, out defaultDictionary);
      }

      Dictionary<string, string?>? variables = payload.Variables == null ? null : MessageHelper.GetVariables(payload.Variables);

      IMessageHandler handler = _messageHandlerFactory.GetHandler(sender);

      Dictionaries? dictionaries = payload.IgnoreUserLocale
        ? MessageHelper.GetDictionaries(payload.Locale, defaultDictionary, allDictionaries)
        : null;

      List<Message> messages = new(capacity: allRecipients.To.Count);
      foreach (Recipient recipient in allRecipients.To)
      {
        Dictionaries? userDictionaries = (!payload.IgnoreUserLocale && recipient.UserLocale != null)
          ? MessageHelper.GetDictionaries(recipient.UserLocale, defaultDictionary, allDictionaries)
          : null;

        string subject = (userDictionaries ?? dictionaries)?.GetEntry(template.Subject) ?? template.Subject;
        string body = _templateCompiler.Compile(template, userDictionaries ?? dictionaries, recipient.User, variables);

        IEnumerable<Recipient> recipients = new[] { recipient }
          .Concat(allRecipients.CC)
          .Concat(allRecipients.Bcc);

        Message message = new(_userContext.ActorId, subject, body, recipients, sender, template, realm, payload.IgnoreUserLocale, payload.IgnoreUserLocale ? payload.Locale : (recipient.UserLocale ?? payload.Locale), variables, isDemo: false);
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

        messages.Add(message);
      }

      await _repository.SaveAsync(messages, cancellationToken);

      return new SentMessagesModel
      {
        Error = messages.Where(x => x.HasErrors).Select(x => x.Id.Value),
        Success = messages.Where(x => x.HasSucceeded).Select(x => x.Id.Value),
        Unsent = messages.Where(x => !x.HasErrors && !x.HasSucceeded).Select(x => x.Id.Value)
      };
    }

    private async Task<Realm?> ResolveRealmAsync(string id, string paramName, CancellationToken cancellationToken)
    {
      Realm realm = await _repository.LoadRealmByAliasOrIdAsync(id, cancellationToken)
        ?? throw new EntityNotFoundException<Realm>(id, paramName);

      return realm;
    }

    private async Task<Recipients> ResolveRecipientsAsync(IEnumerable<RecipientPayload> payloads, Realm? realm, string paramName, CancellationToken cancellationToken)
    {
      List<AggregateId> userIds = new(capacity: payloads.Count());
      List<string> usernames = new(userIds.Capacity);
      foreach (RecipientPayload payload in payloads)
      {
        if (!string.IsNullOrWhiteSpace(payload.User))
        {
          userIds.Add(new AggregateId(payload.User));
          usernames.Add(payload.User);
        }
      }

      Dictionary<AggregateId, User> usersById = (await _repository.LoadAsync<User>(userIds, cancellationToken))
        .ToDictionary(x => x.Id, x => x);
      Dictionary<string, User> usersByUsername = (await _repository.LoadUsersByUsernamesAsync(usernames, realm, cancellationToken))
        .ToDictionary(x => x.Username.ToUpper(), x => x);

      List<string> missingUsers = new(userIds.Capacity);
      List<User> notInRealm = new(userIds.Capacity);
      List<User> missingEmails = new(userIds.Capacity);
      List<Recipient> to = new(userIds.Capacity);
      List<Recipient> cc = new(userIds.Capacity);
      List<Recipient> bcc = new(userIds.Capacity);
      foreach (RecipientPayload recipientPayload in payloads)
      {
        User? user = null;
        if (recipientPayload.User != null)
        {
          if (!usersById.TryGetValue(new AggregateId(recipientPayload.User), out user))
          {
            usersByUsername.TryGetValue(recipientPayload.User.ToUpper(), out user);
          }

          if (user == null)
          {
            missingUsers.Add(recipientPayload.User);
            continue;
          }
          else if (realm?.Id != user.RealmId)
          {
            notInRealm.Add(user);
            continue;
          }
          else if (user.Email == null)
          {
            missingEmails.Add(user);
            continue;
          }
        }

        Recipient recipient = user == null
          ? new(recipientPayload.Address ?? string.Empty, recipientPayload.DisplayName, recipientPayload.Type)
          : new(user, recipientPayload.Type);

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
        throw new UsersNotFoundException(missingUsers, paramName);
      }

      if (notInRealm.Any())
      {
        throw new UsersNotInRealmException(notInRealm, realm, paramName);
      }

      if (missingEmails.Any())
      {
        throw new UsersEmailRequiredException(missingEmails, paramName);
      }

      return new Recipients
      {
        To = to,
        CC = cc,
        Bcc = bcc
      };
    }

    private async Task<Sender> ResolveSenderAsync(string? id, Realm? realm, string paramName, CancellationToken cancellationToken)
    {
      Sender sender;
      if (id != null)
      {
        sender = await _repository.LoadAsync<Sender>(id, cancellationToken)
          ?? throw new EntityNotFoundException<Sender>(id, paramName);
      }
      else
      {
        sender = await _repository.LoadDefaultSenderAsync(realm, cancellationToken)
          ?? throw new DefaultSenderRequiredException(realm);
      }

      if (realm?.Id != sender.RealmId)
      {
        throw new SenderNotInRealmException(sender, realm, paramName);
      }

      return sender;
    }

    private async Task<Template> ResolveTemplateAsync(string id, Realm? realm, string paramName, CancellationToken cancellationToken)
    {
      Template template = await _repository.LoadTemplateByIdOrKeyAsync(id, realm, cancellationToken)
        ?? throw new EntityNotFoundException<Template>(id, paramName);

      if (realm?.Id != template.RealmId)
      {
        throw new TemplateNotInRealmException(template, realm, paramName);
      }

      return template;
    }
  }
}
