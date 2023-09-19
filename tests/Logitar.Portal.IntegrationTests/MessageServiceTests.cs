using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class MessageServiceTests : IntegrationTests, IAsyncLifetime
{
  private const string RecipientKey = "Recipient";
  private const string SenderKey = "Sender";
  private const string SendGridKey = "SendGrid";

  private static readonly ReadOnlyLocale _americanEnglish = new("en-US");
  private static readonly ReadOnlyLocale _canadianEnglish = new("en-CA");
  private static readonly ReadOnlyLocale _english = new("en");
  private static readonly ReadOnlyLocale _french = new("fr");

  private readonly IMessageService _messageService;

  private readonly string _recipient;

  private readonly Dictionary<ReadOnlyLocale, DictionaryAggregate> _dictionaries = new();
  private readonly RealmAggregate _realm;
  private readonly SenderAggregate _sender;
  private readonly TemplateAggregate _template;
  private readonly UserAggregate _user;

  public MessageServiceTests()
  {
    _messageService = ServiceProvider.GetRequiredService<IMessageService>();

    IConfiguration configuration = ServiceProvider.GetRequiredService<IConfiguration>();
    _recipient = configuration.GetValue<string>(RecipientKey)
      ?? throw new InvalidOperationException($"The configuration '{RecipientKey}' is required.");

    _realm = new("logitar")
    {
      DefaultLocale = _french,
      DisplayName = "Logitar",
      RequireUniqueEmail = true
    };
    string tenantId = _realm.Id.Value;

    _sender = CreateSender(isDefault: true, tenantId)
      ?? throw new InvalidOperationException($"The sender could not be created. Please ensure you have the required '{SenderKey}' configuration section and at least one provider settings section.");

    string contents = File.ReadAllText("Templates/PasswordRecovery.cshtml", Encoding.UTF8);
    _template = new(_realm.UniqueNameSettings, "PasswordRecovery", "PasswordRecovery_Subject", MediaTypeNames.Text.Html, contents, tenantId)
    {
      DisplayName = "Password Recovery"
    };

    _user = new(_realm.UniqueNameSettings, Faker.Person.UserName, tenantId)
    {
      Email = new EmailAddress(_recipient),
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = new Gender(Faker.Person.Gender.ToString()),
      Locale = _canadianEnglish,
      Picture = new Uri(Faker.Person.Avatar),
      Website = new Uri($"https://www.{Faker.Person.Website}/")
    };
    Assert.NotNull(_user.Locale.Parent);

    _dictionaries[_americanEnglish] = CreateDictionary(_americanEnglish, tenantId);
    _dictionaries[_canadianEnglish] = CreateDictionary(_canadianEnglish, tenantId);
    _dictionaries[_english] = CreateDictionary(_english, tenantId);
    _dictionaries[_french] = CreateDictionary(_french, tenantId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _sender, _template, _user }.Concat(_dictionaries.Values));
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the message is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_message_is_not_found()
  {
    Assert.Null(await _messageService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return the message found by ID.")]
  public async Task ReadAsync_it_should_return_the_message_found_by_Id()
  {
    Recipients recipients = new(new ReadOnlyRecipient[]
    {
      new(_recipient, Faker.Name.FullName())
    });
    MessageAggregate aggregate = new("Test", "Hello World!", recipients, _sender, _template, _realm);
    await AggregateRepository.SaveAsync(aggregate);

    Message? message = await _messageService.ReadAsync(aggregate.Id.ToGuid());
    Assert.NotNull(message);
    Assert.Equal(aggregate.Id.ToGuid(), message.Id);
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchMessagesPayload payload = new()
    {
      IdIn = new[] { Guid.Empty }
    };

    SearchResults<Message> results = await _messageService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    SenderAggregate sender = new(_sender.EmailAddress, _sender.Provider, isDefault: true)
    {
      DisplayName = _sender.DisplayName
    };

    Assert.NotNull(Configuration);
    TemplateAggregate template = new(Configuration.UniqueNameSettings, _template.UniqueName, _template.Subject, _template.ContentType, _template.Contents)
    {
      DisplayName = _template.DisplayName
    };

    TemplateAggregate createUser = new(_realm.UniqueNameSettings, "CreateUser", "CreateUser_Subject", MediaTypeNames.Text.Html, "CreateUser_Body", _realm.Id.Value)
    {
      DisplayName = "Create User"
    };

    Recipients recipients = new(new ReadOnlyRecipient[]
    {
      new(_recipient, Faker.Name.FullName())
    });
    string body = "Hello World!";

    MessageAggregate notMatching = new("Test", body, recipients, sender, template);
    MessageAggregate idNotIn = new("Reset your password", body, recipients, _sender, _template, _realm);
    MessageAggregate notInRealm = new("Réinitialiser votre mot de passe", body, recipients, sender, template);
    MessageAggregate demo = new("Confirm your account", body, recipients, _sender, _template, _realm, isDemo: true);
    MessageAggregate otherTemplate = new("Confirmer votre compte", body, recipients, _sender, createUser, _realm);
    MessageAggregate message1 = new("Reset your password", body, recipients, _sender, _template, _realm);
    MessageAggregate message2 = new("Réinitialiser votre mot de passe", body, recipients, _sender, _template, _realm);
    MessageAggregate message3 = new("Confirm your account", body, recipients, _sender, _template, _realm);
    MessageAggregate message4 = new("Confirmer votre compte", body, recipients, _sender, _template, _realm);

    MessageAggregate failed = new("Test", "Hello World!", recipients, _sender, _template, _realm, isDemo: true);
    failed.Fail(new SendMessageResult());

    await AggregateRepository.SaveAsync(new AggregateRoot[]
    {
      sender, template, createUser,
      notMatching, idNotIn, notInRealm, demo, otherTemplate, failed,
      message1, message2, message3, message4
    });

    MessageAggregate[] messages = new[] { message1, message2, message3, message4 }
      .OrderBy(x => x.Subject.Replace('é', 'e').Replace(" ", null)).Skip(1).Take(2).ToArray();

    HashSet<Guid> ids = (await PortalContext.Messages.AsNoTracking().ToArrayAsync())
      .Select(message => new AggregateId(message.AggregateId).ToGuid()).ToHashSet();
    ids.Remove(idNotIn.Id.ToGuid());

    SearchMessagesPayload payload = new()
    {
      Search = new TextSearch
      {
        Operator = SearchOperator.Or,
        Terms = new SearchTerm[]
       {
          new("%your%"),
          new("%votre%"),
          new(Guid.NewGuid().ToString())
       }
      },
      IdIn = ids,
      Realm = $" {_realm.UniqueSlug.ToUpper()} ",
      IsDemo = false,
      Status = MessageStatus.Unsent,
      Template = $" {_template.UniqueName} ",
      Sort = new MessageSortOption[]
      {
        new(MessageSort.Subject)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Message> results = await _messageService.SearchAsync(payload);

    Assert.Equal(messages.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < messages.Length; i++)
    {
      Assert.Equal(messages[i].Id.ToGuid(), results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "SendAsync: it should send a message to a Portal user.")]
  public async Task SendAsync_it_should_send_a_message_to_a_Portal_user()
  {
    SenderAggregate sender = new(_sender.EmailAddress, _sender.Provider, isDefault: true)
    {
      DisplayName = _sender.DisplayName
    };
    foreach (KeyValuePair<string, string> setting in _sender.Settings)
    {
      sender.SetSetting(setting.Key, setting.Value);
    }

    Assert.NotNull(Configuration);
    TemplateAggregate template = new(Configuration.UniqueNameSettings, _template.UniqueName, _template.Subject, _template.ContentType, _template.Contents)
    {
      DisplayName = _template.DisplayName
    };

    Assert.NotNull(User);
    SetUserEmail(_recipient);

    await AggregateRepository.SaveAsync(new AggregateRoot[] { sender, template, User });

    SendMessagePayload payload = new()
    {
      Template = template.Id.ToGuid().ToString(),
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = User.Id.ToGuid().ToString()
        }
      }
    };

    SentMessages sentMessages = await _messageService.SendAsync(payload);
    AggregateId messageId = new(Assert.Single(sentMessages.Ids));

    MessageAggregate? message = await AggregateRepository.LoadAsync<MessageAggregate>(messageId);
    Assert.NotNull(message);
    Assert.Equal(messageId, message.Id);

    Assert.Equal(template.Subject, message.Subject);
    Assert.Null(message.Realm);
    Assert.Equal(SenderSummary.From(sender), message.Sender);
    Assert.Equal(TemplateSummary.From(template), message.Template);
    Assert.False(message.IgnoreUserLocale);
    Assert.Equal(User.Locale, message.Locale);
    Assert.Empty(message.Variables.AsDictionary());
    Assert.False(message.IsDemo);

    string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" lang=""{locale}"" xml:lang=""{locale}"">
<head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
</head>
<body>
  <p><strong>PasswordRecovery_Hello</strong></p>
  <p>PasswordRecovery_PasswordLost</p>
  <p>
    PasswordRecovery_ClickLink
    <br />
    <a href=""PasswordRecovery_PageUrl"">PasswordRecovery_PageUrl</a>
  </p>
  <p>PasswordRecovery_OtherwiseDelete</p>
  <p>
    Cordially
    <br />
    <i>Team</i>
  </p>
</body>
</html>".Replace("{locale}", User.Locale?.Code);
    Assert.Equal(body, message.Body);

    ReadOnlyRecipient recipient = Assert.Single(message.Recipients.AsEnumerable());
    Assert.Equal(RecipientType.To, recipient.Type);
    Assert.Equal(User.Email?.Address, recipient.Address);
    Assert.Equal(User.FullName, recipient.DisplayName);
    Assert.Equal(User.Id, recipient.UserId);

    if (message.Status == MessageStatus.Unsent)
    {
      Assert.Null(message.Result);
    }
    else
    {
      Assert.NotNull(message.Result);
    }
  }

  [Fact(DisplayName = "SendAsync: it should send a message to a realm user.")]
  public async Task SendAsync_it_should_send_a_message_to_a_realm_user()
  {
    string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToUriSafeBase64();
    SendMessagePayload payload = new()
    {
      Realm = $"  {_realm.UniqueSlug}  ",
      SenderId = _sender.Id.ToGuid(),
      Template = $"  {_template.UniqueName}  ",
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = $"  {_user.UniqueName}  "
        }
      },
      IgnoreUserLocale = true,
      Locale = "  en-US  ",
      Variables = new Variable[]
      {
        new("Token", token)
      }
    };

    SentMessages sentMessages = await _messageService.SendAsync(payload);
    AggregateId messageId = new(Assert.Single(sentMessages.Ids));

    MessageAggregate? message = await AggregateRepository.LoadAsync<MessageAggregate>(messageId);
    Assert.NotNull(message);
    Assert.Equal(messageId, message.Id);
    Assert.Equal(ActorId, message.CreatedBy);
    AssertIsNear(message.CreatedOn);
    Assert.Equal(ActorId, message.UpdatedBy);
    AssertIsNear(message.UpdatedOn);
    Assert.True(message.Version > 1);

    Assert.Equal("Reset your password", message.Subject);
    Assert.Equal(RealmSummary.From(_realm), message.Realm);
    Assert.Equal(SenderSummary.From(_sender), message.Sender);
    Assert.Equal(TemplateSummary.From(_template), message.Template);
    Assert.True(message.IgnoreUserLocale);
    Assert.Equal(payload.Locale.Trim(), message.Locale?.Code);
    Assert.Equal(payload.Variables.ToDictionary(x => x.Key, x => x.Value), message.Variables.AsDictionary());
    Assert.False(message.IsDemo);

    string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" lang=""en-CA"" xml:lang=""en-CA"">
<head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
</head>
<body>
  <p><strong>Bonjour {name} !</strong></p>
  <p>It seems you have lost your password...</p>
  <p>
    In this case, please click on the link below to reset it:
    <br />
    <a href=""https://www.logitar.com/en-us/user/reset-password?token={token}"">https://www.logitar.com/en-us/user/reset-password?token={token}</a>
  </p>
  <p>If we&#39;ve been mistaken, we suggest you to delete this message.</p>
  <p>
    Cordially,
    <br />
    <i>The Logitar Team</i>
  </p>
</body>
</html>".Replace("{name}", _user.FullName).Replace("{token}", token);
    Assert.Equal(body, message.Body);

    ReadOnlyRecipient recipient = Assert.Single(message.Recipients.AsEnumerable());
    Assert.Equal(RecipientType.To, recipient.Type);
    Assert.Equal(_user.Email?.Address, recipient.Address);
    Assert.Equal(_user.FullName, recipient.DisplayName);
    Assert.Equal(_user.Id, recipient.UserId);

    if (message.Status == MessageStatus.Unsent)
    {
      Assert.Null(message.Result);
    }
    else
    {
      Assert.NotNull(message.Result);
    }
  }

  [Fact(DisplayName = "SendAsync: it should send a message to a recipient who is not an user.")]
  public async Task SendAsync_it_should_send_a_message_to_a_recipient_who_is_not_an_user()
  {
    SendMessagePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Template = _template.UniqueName,
      Recipients = new RecipientPayload[]
      {
        new()
        {
          Address = _recipient,
          DisplayName = Faker.Name.FullName()
        }
      }
    };

    SentMessages sentMessages = await _messageService.SendAsync(payload);
    AggregateId messageId = new(Assert.Single(sentMessages.Ids));

    MessageAggregate? message = await AggregateRepository.LoadAsync<MessageAggregate>(messageId);
    Assert.NotNull(message);
    Assert.Equal(messageId, message.Id);
    Assert.Equal(ActorId, message.CreatedBy);
    AssertIsNear(message.CreatedOn);
    Assert.Equal(ActorId, message.UpdatedBy);
    AssertIsNear(message.UpdatedOn);
    Assert.True(message.Version > 1);

    Assert.Equal("Réinitialiser votre mot de passe", message.Subject);
    Assert.Equal(RealmSummary.From(_realm), message.Realm);
    Assert.Equal(SenderSummary.From(_sender), message.Sender);
    Assert.Equal(TemplateSummary.From(_template), message.Template);
    Assert.False(message.IgnoreUserLocale);
    Assert.Null(message.Locale);
    Assert.Empty(message.Variables.AsDictionary());
    Assert.False(message.IsDemo);

    string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
</head>
<body>
  <p><strong>Bonjour  !</strong></p>
  <p>Vous semblez avoir perdu votre mot de passe...</p>
  <p>
    Si c’est bien le cas, veuillez cliquer sur le lien ci-dessous afin de le r&#233;initialiser :
    <br />
    <a href=""https://www.logitar.ca/fr/compte/reinitialiser-mot-de-passe?token=Token"">https://www.logitar.ca/fr/compte/reinitialiser-mot-de-passe?token=Token</a>
  </p>
  <p>Si nous avons fait erreur, veuillez supprimer ce message.</p>
  <p>
    Cordialement,
    <br />
    <i>L’&#233;quipe Logitar</i>
  </p>
</body>
</html>";
    Assert.Equal(body, message.Body);

    RecipientPayload recipientPayload = Assert.Single(payload.Recipients);
    ReadOnlyRecipient recipient = Assert.Single(message.Recipients.AsEnumerable());
    Assert.Equal(RecipientType.To, recipient.Type);
    Assert.Equal(recipientPayload.Address, recipient.Address);
    Assert.Equal(recipientPayload.DisplayName, recipient.DisplayName);
    Assert.Null(recipient.UserId);

    if (message.Status == MessageStatus.Unsent)
    {
      Assert.Null(message.Result);
    }
    else
    {
      Assert.NotNull(message.Result);
    }
  }

  [Fact(DisplayName = "SendAsync: it should send a message to multiple recipients.")]
  public async Task SendAsync_it_should_send_a_message_to_multiple_recipients()
  {
    Assert.NotNull(User);
    Assert.NotNull(_user.Email);
    string displayName = Faker.Name.FullName();
    SendMessagePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Template = _template.UniqueName,
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = User.Id.ToGuid().ToString()
        },
        new()
        {
          User = _user.Email.Address
        },
        new()
        {
          Address = _recipient,
          DisplayName = displayName
        }
      },
      Locale = _realm.DefaultLocale?.Code
    };

    SentMessages sentMessages = await _messageService.SendAsync(payload);

    HashSet<AggregateId> messageIds = sentMessages.Ids.Select(id => new AggregateId(id)).ToHashSet();
    Assert.Equal(payload.Recipients.Count(), messageIds.Count);

    IEnumerable<MessageAggregate> messages = await AggregateRepository.LoadAsync<MessageAggregate>(messageIds);
    Assert.Equal(messageIds.Count, messages.Count());
    Assert.Contains(messages, message => message.Recipients.To.Single().UserId == _user.Id && message.Locale == _user.Locale);
    Assert.Contains(messages, message => message.Recipients.To.Single().Address == _recipient && message.Recipients.To.Single().DisplayName == displayName
      && !message.Recipients.To.Single().UserId.HasValue && message.Locale == _realm.DefaultLocale);
  }

  [Fact(DisplayName = "SendAsync: it should throw AggregateNotFoundException when the realm could not be found.")]
  public async Task SendAsync_it_should_throw_AggregateNotFoundException_when_the_realm_could_not_be_found()
  {
    SendMessagePayload payload = new()
    {
      Realm = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "SendAsync: it should throw AggregateNotFoundException when the sender could not be found.")]
  public async Task SendAsync_it_should_throw_AggregateNotFoundException_when_the_sender_could_not_be_found()
  {
    SendMessagePayload payload = new()
    {
      SenderId = Guid.Empty
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<SenderAggregate>>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(payload.SenderId.ToString(), exception.Id);
    Assert.Equal(nameof(payload.SenderId), exception.PropertyName);
  }

  [Fact(DisplayName = "SendAsync: it should throw AggregateNotFoundException when the template could not be found.")]
  public async Task SendAsync_it_should_throw_AggregateNotFoundException_when_the_template_could_not_be_found()
  {
    SendMessagePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Template = Guid.Empty.ToString()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<TemplateAggregate>>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(payload.Template.ToString(), exception.Id);
    Assert.Equal(nameof(payload.Template), exception.PropertyName);
  }

  [Fact(DisplayName = "SendAsync: it should throw MissingRecipientAddressesException when some recipients are missing an email address.")]
  public async Task SendAsync_it_should_throw_MissingRecipientAddressesException_when_some_recipients_are_missing_an_email_address()
  {
    UserAggregate user = new(_realm.UniqueNameSettings, Faker.Internet.UserName(), _realm.Id.Value)
    {
      FirstName = Faker.Name.FirstName(),
      LastName = Faker.Name.LastName()
    };
    await AggregateRepository.SaveAsync(user);

    SendMessagePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Template = _template.UniqueName,
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = user.UniqueName
        },
        new()
        {
          Type = RecipientType.Bcc,
          DisplayName = Faker.Name.FullName()
        }
      }
    };

    var exception = await Assert.ThrowsAsync<MissingRecipientAddressesException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(new string[]
    {
      $"Recipients[0].User:{user.Id.ToGuid()}",
      "Recipients[1].Address"
    }, exception.Recipients);
    Assert.Equal(nameof(payload.Recipients), exception.PropertyName);
  }

  [Fact(DisplayName = "SendAsync: it should throw RealmHasNoDefaultSenderException when the realm has no default sender.")]
  public async Task SendAsync_it_should_throw_RealmHasNoDefaultSenderException_when_the_realm_has_not_default_sender()
  {
    RealmAggregate realm = new("desjardins")
    {
      DefaultLocale = new ReadOnlyLocale("fr-CA"),
      DisplayName = "Desjardins",
      Url = new Uri("https://www.desjardins.com/")
    };
    await AggregateRepository.SaveAsync(realm);

    SendMessagePayload payload = new()
    {
      Realm = realm.UniqueSlug
    };

    var exception = await Assert.ThrowsAsync<RealmHasNoDefaultSenderException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(realm.ToString(), exception.Realm);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "SendAsync: it should throw SenderNotInRealmException when the sender is not in the realm.")]
  public async Task SendAsync_it_should_throw_SenderNotInRealmException_when_the_sender_is_not_in_the_realm()
  {
    Assert.NotNull(Configuration);
    TemplateAggregate template = new(Configuration.UniqueNameSettings, _template.UniqueName, _template.Subject, _template.ContentType, _template.Contents)
    {
      DisplayName = _template.DisplayName
    };
    await AggregateRepository.SaveAsync(template);

    Assert.NotNull(User);
    SendMessagePayload payload = new()
    {
      SenderId = _sender.Id.ToGuid(),
      Template = template.Id.ToGuid().ToString(),
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = User.Id.ToGuid().ToString()
        }
      }
    };

    var exception = await Assert.ThrowsAsync<SenderNotInRealmException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(_sender.ToString(), exception.Sender);
    Assert.Null(exception.Realm);
    Assert.Equal(nameof(MessageAggregate.Sender), exception.PropertyName);
  }

  [Fact(DisplayName = "SendAsync: it should throw TemplateNotInRealmException when the template is not in the realm.")]
  public async Task SendAsync_it_should_throw_TemplateNotInRealmException_when_the_template_is_not_in_the_realm()
  {
    Assert.NotNull(Configuration);
    TemplateAggregate template = new(Configuration.UniqueNameSettings, _template.UniqueName, _template.Subject, _template.ContentType, _template.Contents)
    {
      DisplayName = _template.DisplayName
    };
    await AggregateRepository.SaveAsync(template);

    SendMessagePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Template = template.Id.ToGuid().ToString(),
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = _user.Id.ToGuid().ToString()
        }
      }
    };

    var exception = await Assert.ThrowsAsync<TemplateNotInRealmException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(template.ToString(), exception.Template);
    Assert.Equal(_realm.ToString(), exception.Realm);
    Assert.Equal(nameof(payload.Template), exception.PropertyName);
  }

  [Fact(DisplayName = "SendAsync: it should throw UsersNotFoundException when some users could not be found.")]
  public async Task SendAsync_it_should_throw_UsersNotFoundException_when_some_users_could_not_be_found()
  {
    Assert.NotNull(User);
    SendMessagePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Template = _template.UniqueName,
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = User.Id.ToGuid().ToString()
        },
        new()
        {
          User = _user.Id.ToGuid().ToString()
        },
        new()
        {
          User = Guid.Empty.ToString()
        }
      }
    };

    var exception = await Assert.ThrowsAsync<UsersNotFoundException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(Guid.Empty.ToString(), exception.MissingUsers.Single());
  }

  [Fact(DisplayName = "SendAsync: it should throw UsersNotInRealmException when some users are not in the realm.")]
  public async Task SendAsync_it_should_throw_UsersNotInRealmException_when_some_users_are_not_in_the_realm()
  {
    RealmAggregate realm = new($"{_realm.UniqueSlug}-2");
    UserAggregate user = new(realm.UniqueNameSettings, Faker.Person.UserName, realm.Id.Value)
    {
      Email = new EmailAddress(_recipient)
    };
    await AggregateRepository.SaveAsync(new AggregateRoot[] { realm, user });

    Assert.NotNull(User);
    SendMessagePayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Template = _template.UniqueName,
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = User.Id.ToGuid().ToString()
        },
        new()
        {
          User = _user.Id.ToGuid().ToString()
        },
        new()
        {
          User = user.Id.ToGuid().ToString()
        }
      }
    };

    var exception = await Assert.ThrowsAsync<UsersNotInRealmException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(user.ToString(), exception.Users.Single());
    Assert.Equal(_realm.ToString(), exception.Realm);
    Assert.Equal(nameof(payload.Recipients), exception.PropertyName);
  }

  [Fact(DisplayName = "SendDemoAsync: it should send a demo message with all arguments.")]
  public async Task SendDemoAsync_it_should_send_a_demo_message_with_all_arguments()
  {
    Assert.NotNull(User);
    SetUserEmail(_recipient);

    SenderAggregate sender = new(_sender.EmailAddress, _sender.Provider, isDefault: false, _sender.TenantId)
    {
      DisplayName = Faker.Name.FullName()
    };
    foreach (KeyValuePair<string, string> setting in _sender.Settings)
    {
      sender.SetSetting(setting.Key, setting.Value);
    }

    await AggregateRepository.SaveAsync(new AggregateRoot[] { User, sender });

    string token = "9dFny0oyBUqd2e4ESh_xzQ";
    SendDemoMessagePayload payload = new()
    {
      SenderId = sender.Id.ToGuid(),
      TemplateId = _template.Id.ToGuid(),
      Locale = "  en-US  ",
      Variables = new Variable[]
      {
        new("Token", token)
      }
    };

    Message message = await _messageService.SendDemoAsync(payload);

    Assert.NotEqual(Guid.Empty, message.Id);
    Assert.Equal(ActorId.ToGuid(), message.CreatedBy.Id);
    AssertIsNear(message.CreatedOn);
    Assert.Equal(message.CreatedBy, message.UpdatedBy);
    Assert.True(message.CreatedOn < message.UpdatedOn);
    Assert.Equal(2, message.Version);

    Assert.Equal("Reset your password", message.Subject);
    Assert.Equal(_realm.Id.ToGuid(), message.Realm?.Id);
    Assert.Equal(sender.Id.ToGuid(), message.Sender.Id);
    Assert.Equal(_template.Id.ToGuid(), message.Template.Id);
    Assert.True(message.IgnoreUserLocale);
    Assert.Equal(payload.Locale.Trim(), message.Locale?.Code);
    Assert.Equal(payload.Variables, message.Variables);
    Assert.True(message.IsDemo);

    string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" lang=""en"" xml:lang=""en"">
<head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
</head>
<body>
  <p><strong>Bonjour {name} !</strong></p>
  <p>It seems you have lost your password...</p>
  <p>
    In this case, please click on the link below to reset it:
    <br />
    <a href=""https://www.logitar.com/en-us/user/reset-password?token={token}"">https://www.logitar.com/en-us/user/reset-password?token={token}</a>
  </p>
  <p>If we&#39;ve been mistaken, we suggest you to delete this message.</p>
  <p>
    Cordially,
    <br />
    <i>The Logitar Team</i>
  </p>
</body>
</html>".Replace("{name}", User.FullName).Replace("{token}", token);
    Assert.Equal(body, message.Body);

    Assert.Equal(1, message.RecipientCount);
    Recipient recipient = Assert.Single(message.Recipients);
    Assert.Equal(RecipientType.To, recipient.Type);
    Assert.Equal(User.Id.ToGuid(), recipient.User?.Id);
    Assert.Equal(User.Email?.Address, recipient.Address);
    Assert.Equal(User.FullName, recipient.DisplayName);

    if (message.Status == MessageStatus.Unsent)
    {
      Assert.Null(message.Result);
    }
    else
    {
      Assert.NotNull(message.Result);
    }
  }

  [Fact(DisplayName = "SendDemoAsync: it should send a demo message with default arguments.")]
  public async Task SendDemoAsync_it_should_send_a_demo_message_with_default_arguments()
  {
    Assert.NotNull(User);
    SetUserEmail(_recipient);
    await AggregateRepository.SaveAsync(User);

    SendDemoMessagePayload payload = new()
    {
      TemplateId = _template.Id.ToGuid()
    };

    Message message = await _messageService.SendDemoAsync(payload);

    Assert.NotEqual(Guid.Empty, message.Id);
    Assert.Equal(ActorId.ToGuid(), message.CreatedBy.Id);
    AssertIsNear(message.CreatedOn);
    Assert.Equal(message.CreatedBy, message.UpdatedBy);
    Assert.True(message.CreatedOn < message.UpdatedOn);
    Assert.Equal(2, message.Version);

    Assert.Equal("Reset your password", message.Subject);
    Assert.Equal(_realm.Id.ToGuid(), message.Realm?.Id);
    Assert.Equal(_sender.Id.ToGuid(), message.Sender.Id);
    Assert.Equal(_template.Id.ToGuid(), message.Template.Id);
    Assert.False(message.IgnoreUserLocale);
    Assert.Equal(User.Locale?.Code, message.Locale?.Code);
    Assert.Empty(message.Variables);
    Assert.True(message.IsDemo);

    string body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" lang=""en"" xml:lang=""en"">
<head>
  <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
</head>
<body>
  <p><strong>Bonjour {name} !</strong></p>
  <p>It seems you have lost your password...</p>
  <p>
    In this case, please click on the link below to reset it:
    <br />
    <a href=""https://www.logitar.com/en/user/reset-password?token=Token"">https://www.logitar.com/en/user/reset-password?token=Token</a>
  </p>
  <p>If we&#39;ve been mistaken, we suggest you to delete this message.</p>
  <p>
    Cordially,
    <br />
    <i>The Logitar Team</i>
  </p>
</body>
</html>".Replace("{name}", User.FullName);
    Assert.Equal(body, message.Body);

    Assert.Equal(1, message.RecipientCount);
    Recipient recipient = Assert.Single(message.Recipients);
    Assert.Equal(RecipientType.To, recipient.Type);
    Assert.Equal(User.Id.ToGuid(), recipient.User?.Id);
    Assert.Equal(User.Email?.Address, recipient.Address);
    Assert.Equal(User.FullName, recipient.DisplayName);

    if (message.Status == MessageStatus.Unsent)
    {
      Assert.Null(message.Result);
    }
    else
    {
      Assert.NotNull(message.Result);
    }
  }

  [Fact(DisplayName = "SendDemoAsync: it should throw AggregateNotFoundException when the sender could not be found.")]
  public async Task SendDemoAsync_it_should_throw_AggregateNotFoundException_when_the_sender_could_not_be_found()
  {
    SendDemoMessagePayload payload = new()
    {
      SenderId = Guid.Empty,
      TemplateId = _template.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<SenderAggregate>>(async () => await _messageService.SendDemoAsync(payload));
    Assert.Equal(payload.SenderId.ToString(), exception.Id);
    Assert.Equal(nameof(payload.SenderId), exception.PropertyName);
  }

  [Fact(DisplayName = "SendDemoAsync: it should throw AggregateNotFoundException when the template could not be found.")]
  public async Task SendDemoAsync_it_should_throw_AggregateNotFoundException_when_the_template_could_not_be_found()
  {
    SendDemoMessagePayload payload = new()
    {
      TemplateId = Guid.Empty
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<TemplateAggregate>>(async () => await _messageService.SendDemoAsync(payload));
    Assert.Equal(payload.TemplateId.ToString(), exception.Id);
    Assert.Equal(nameof(payload.TemplateId), exception.PropertyName);
  }

  [Fact(DisplayName = "SendDemoAsync: it should throw AggregateNotFoundException when the user could not be found.")]
  public async Task SendDemoAsync_it_should_throw_AggregateNotFoundException_when_the_user_could_not_be_found()
  {
    Assert.NotNull(Session);
    Session.Delete(ActorId);
    Assert.NotNull(User);
    User.Delete(ActorId);
    await AggregateRepository.SaveAsync(new AggregateRoot[] { Session, User });

    SendDemoMessagePayload payload = new()
    {
      TemplateId = _template.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<UserAggregate>>(async () => await _messageService.SendDemoAsync(payload));
    Assert.Equal(User.Id.Value, exception.Id);
    Assert.Equal(nameof(IApplicationContext.ActorId), exception.PropertyName);
  }

  [Fact(DisplayName = "SendDemoAsync: it should throw MissingRecipientAddressesException when the user has no email address.")]
  public async Task SendDemoAsync_it_should_throw_MissingRecipientAddressesException_when_the_user_has_no_email_address()
  {
    Assert.NotNull(User);
    SetUserEmail(address: null);
    await AggregateRepository.SaveAsync(User);

    SendDemoMessagePayload payload = new()
    {
      TemplateId = _template.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<MissingRecipientAddressesException>(async () => await _messageService.SendDemoAsync(payload));
    Assert.Equal(new[] { $"Recipients[0].User:{User.Id.ToGuid()}" }, exception.Recipients);
    Assert.Equal(nameof(SendMessagePayload.Recipients), exception.PropertyName);
  }

  [Fact(DisplayName = "SendDemoAsync: it should throw RealmHasNoDefaultSenderException when the realm has no default sender.")]
  public async Task SendDemoAsync_it_should_throw_RealmHasNoDefaultSenderException_when_the_realm_has_not_default_sender()
  {
    Assert.NotNull(Configuration);
    TemplateAggregate template = new(Configuration.UniqueNameSettings, _template.UniqueName, _template.Subject, _template.ContentType, _template.Contents)
    {
      DisplayName = _template.DisplayName
    };
    await AggregateRepository.SaveAsync(template);

    SendDemoMessagePayload payload = new()
    {
      TemplateId = template.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<RealmHasNoDefaultSenderException>(async () => await _messageService.SendDemoAsync(payload));
    Assert.Null(exception.Realm);
    Assert.Equal(nameof(SendMessagePayload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "SendDemoAsync: it should throw SenderNotInRealmException when the sender is not in the realm.")]
  public async Task SendDemoAsync_it_should_throw_SenderNotInRealmException_when_the_sender_is_not_in_the_realm()
  {
    Assert.NotNull(Configuration);
    TemplateAggregate template = new(Configuration.UniqueNameSettings, _template.UniqueName, _template.Subject, _template.ContentType, _template.Contents)
    {
      DisplayName = _template.DisplayName
    };
    await AggregateRepository.SaveAsync(template);

    SendDemoMessagePayload payload = new()
    {
      SenderId = _sender.Id.ToGuid(),
      TemplateId = template.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<SenderNotInRealmException>(async () => await _messageService.SendDemoAsync(payload));
    Assert.Equal(_sender.ToString(), exception.Sender);
    Assert.Null(exception.Realm);
    Assert.Equal(nameof(MessageAggregate.Sender), exception.PropertyName);
  }

  private static DictionaryAggregate CreateDictionary(ReadOnlyLocale locale, string? tenantId = null)
  {
    string json = File.ReadAllText($"Dictionaries/{locale.Code}.json", Encoding.UTF8);
    Dictionary<string, string> entries = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();

    DictionaryAggregate dictionary = new(locale, tenantId);
    foreach (KeyValuePair<string, string> entry in entries)
    {
      dictionary.SetEntry(entry.Key, entry.Value);
    }

    return dictionary;
  }

  private SenderAggregate? CreateSender(bool isDefault = false, string? tenantId = null)
  {
    IConfiguration configuration = ServiceProvider.GetRequiredService<IConfiguration>();
    SenderSettings? settings = configuration.GetSection(SenderKey).Get<SenderSettings>();
    if (settings == null)
    {
      return null;
    }

    SendGridSettings? sendGrid = configuration.GetSection(SendGridKey).Get<SendGridSettings>();
    if (sendGrid != null)
    {
      SenderAggregate sender = new(settings.Address, ProviderType.SendGrid, isDefault, tenantId)
      {
        DisplayName = settings.DisplayName ?? "Logitar.Portal.IntegrationTests"
      };
      sender.SetSetting(nameof(sendGrid.ApiKey), sendGrid.ApiKey);

      return sender;
    }

    return null;
  }

  private void SetUserEmail(string? address)
  {
    if (User != null)
    {
      User.Email = address == null ? null : new EmailAddress(address);
    }
  }
}
