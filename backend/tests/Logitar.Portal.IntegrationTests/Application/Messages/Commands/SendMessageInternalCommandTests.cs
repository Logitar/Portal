using FluentValidation;
using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Messages.Settings;
using Logitar.Portal.Application.OneTimePasswords.Commands;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Tokens.Commands;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Senders.Twilio;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Messages.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SendMessageInternalCommandTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IMessageRepository _messageRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;
  private readonly IUserRepository _userRepository;

  private readonly RecipientSettings _recipientSettings;
  private readonly SenderSettings _senderSettings;

  private SenderAggregate? _sender = null;
  private Template? _template = null;

  public SendMessageInternalCommandTests() : base()
  {
    _dictionaryRepository = ServiceProvider.GetRequiredService<IDictionaryRepository>();
    _messageRepository = ServiceProvider.GetRequiredService<IMessageRepository>();
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

    _recipientSettings = ServiceProvider.GetRequiredService<IConfiguration>().GetSection("Recipient").Get<RecipientSettings>() ?? new();
    _senderSettings = ServiceProvider.GetRequiredService<IConfiguration>().GetSection("Sender").Get<SenderSettings>() ?? new();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Messages.Table, PortalDb.Senders.Table, PortalDb.Templates.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should send a message with Mailgun.")]
  public async Task It_should_send_a_message_with_Mailgun()
  {
    await It_should_send_a_message_with_a_provider(SenderProvider.Mailgun, ignoreUserLocale: false);
  }
  [Fact(DisplayName = "It should send a message with SendGrid.")]
  public async Task It_should_send_a_message_with_SendGrid()
  {
    await It_should_send_a_message_with_a_provider(SenderProvider.SendGrid);
  }
  [Fact(DisplayName = "It should send a message with Twilio.")]
  public async Task It_should_send_a_message_with_Twilio()
  {
    await It_should_send_a_message_with_a_provider(SenderProvider.Twilio);
  }
  private async Task It_should_send_a_message_with_a_provider(SenderProvider provider, bool ignoreUserLocale = true)
  {
    SenderType type = provider.GetSenderType();

    Assert.NotNull(Realm.DefaultLocale);
    SetRealm();

    Locale locale = await CreateDictionariesAsync(TenantId);

    await CreateSenderAsync(TenantId, provider);
    Assert.NotNull(_sender);

    await CreateTemplateAsync(TenantId, provider);
    Assert.NotNull(_template);

    UniqueName uniqueName = new(Realm.UniqueNameSettings, _recipientSettings.Address);
    UserAggregate user = new(uniqueName, TenantId);
    user.SetEmail(new EmailUnit(_recipientSettings.Address, isVerified: false));
    user.SetPhone(new PhoneUnit(_recipientSettings.PhoneNumber, countryCode: null, extension: null, isVerified: false));
    string[] names = _recipientSettings.DisplayName?.Split() ?? [];
    if (names.Length > 0)
    {
      user.FirstName = new PersonNameUnit(names.First());
    }
    if (names.Length > 1)
    {
      user.LastName = new PersonNameUnit(names.Last());
    }
    if (names.Length > 2)
    {
      user.MiddleName = new PersonNameUnit(string.Join(' ', names.Skip(1).Take(names.Length - 2)));
    }
    user.Locale = ignoreUserLocale ? new Locale(Realm.DefaultLocale.Code) : locale;
    user.Update();
    await _userRepository.SaveAsync(user);

    SendMessagePayload payload = new("PasswordRecovery")
    {
      IgnoreUserLocale = ignoreUserLocale,
      Locale = locale.Code,
      IsDemo = true
    };
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      UserId = user.Id.ToGuid()
    });

    switch (type)
    {
      case SenderType.Email:
        CreateTokenPayload createToken = new()
        {
          IsConsumable = true,
          Type = "reset_password+jwt",
          Subject = user.Id.ToGuid().ToString()
        };
        CreatedToken createdToken = await ActivityPipeline.ExecuteAsync(new CreateTokenCommand(createToken));
        payload.Variables.Add(new Variable("Token", createdToken.Token));
        break;
      case SenderType.Sms:
        CreateOneTimePasswordPayload createOneTimePassword = new("0123456789", length: 6)
        {
          ExpiresOn = DateTime.Now.AddHours(1),
          MaximumAttempts = 5
        };
        createOneTimePassword.CustomAttributes.Add(new("Purpose", "PasswordRecovery"));
        createOneTimePassword.CustomAttributes.Add(new("UserId", user.Id.Value));
        OneTimePasswordModel oneTimePassword = await ActivityPipeline.ExecuteAsync(new CreateOneTimePasswordCommand(createOneTimePassword));
        Assert.NotNull(oneTimePassword.Password);
        payload.Variables.Add(new Variable("Code", oneTimePassword.Password));
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }


    SendMessageInternalCommand command = new(payload);
    SentMessages sentMessages = await ActivityPipeline.ExecuteAsync(command);

    Guid id = Assert.Single(sentMessages.Ids);
    MessageAggregate? message = await _messageRepository.LoadAsync(id);
    Assert.NotNull(message);

    Assert.Equal(TenantId, message.TenantId);
    Assert.Equal("Reset your password", message.Subject.Value);
    Assert.Equal(_template.Content.Type, message.Body.Type);
    Assert.DoesNotContain("Model.", message.Body.Text);

    switch (type)
    {
      case SenderType.Email:
        Assert.Contains($"Bonjour {user.FullName} !", message.Body.Text);
        Assert.Contains("L&#39;&#233;quipe Logitar", message.Body.Text);
        break;
      case SenderType.Sms:
        string code = Assert.Single(payload.Variables, v => v.Key == "Code").Value;
        Assert.Equal($"Your password reset code is {code}. This code is only valid for one hour.", message.Body.Text);
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }

    Assert.Equal(_sender.Id, message.Sender.Id);
    Assert.Equal(_template.Id, message.Template.Id);
    Assert.Equal(ignoreUserLocale, message.IgnoreUserLocale);
    Assert.Equal(locale, message.Locale);
    Assert.True(message.IsDemo);
    Assert.Equal(MessageStatus.Succeeded, message.Status);

    RecipientUnit recipient = Assert.Single(message.Recipients);
    Assert.Equal(RecipientType.To, recipient.Type);
    Assert.Equal(_recipientSettings.Address, recipient.Address);
    Assert.Equal(_recipientSettings.DisplayName, recipient.DisplayName);
    Assert.Equal(_recipientSettings.PhoneNumber, recipient.PhoneNumber);
    Assert.Equal(user.Id, recipient.UserId);

    Assert.Equal(payload.Variables.Count, message.Variables.Count);
    foreach (Variable variable in payload.Variables)
    {
      Assert.Contains(message.Variables, v => v.Key == variable.Key && v.Value == variable.Value);
    }
  }

  [Fact(DisplayName = "It should throw InvalidSmsMessageContentTypeException when sending an HTML SMS message.")]
  public async Task It_should_throw_InvalidSmsMessageContentTypeException_when_sending_an_Html_Sms_message()
  {
    await CreateTemplateAsync();
    await CreateSenderAsync(provider: SenderProvider.Twilio);

    SendMessagePayload payload = new("PasswordRecovery");
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      PhoneNumber = Faker.Phone.PhoneNumber()
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidSmsMessageContentTypeException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("text/html", exception.ContentType);
    Assert.Equal("Template", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw MissingRecipientContactsException when an user is missing a phone.")]
  public async Task It_should_throw_MissingRecipientContactsException_when_an_user_is_missing_a_phone()
  {
    await CreateSenderAsync(TenantId, SenderProvider.Twilio);

    UserAggregate user = new(new UniqueName(Realm.UniqueNameSettings, UsernameString), TenantId);
    await _userRepository.SaveAsync(user);

    SetRealm();

    SendMessagePayload payload = new("PasswordRecovery");
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      UserId = user.Id.ToGuid()
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<MissingRecipientContactsException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal([user.Id.ToGuid()], exception.UserIds);
    Assert.Equal("Recipients", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw MissingRecipientContactsException when an user is missing an email.")]
  public async Task It_should_throw_MissingRecipientContactsException_when_an_user_is_missing_an_email()
  {
    await CreateSenderAsync(TenantId);

    UserAggregate user = new(new UniqueName(Realm.UniqueNameSettings, UsernameString), TenantId);
    await _userRepository.SaveAsync(user);

    SetRealm();

    SendMessagePayload payload = new("PasswordRecovery");
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      UserId = user.Id.ToGuid()
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<MissingRecipientContactsException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal([user.Id.ToGuid()], exception.UserIds);
    Assert.Equal("Recipients", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw NoDefaultSenderException when the realm has no sender.")]
  public async Task It_should_throw_NoDefaultSenderException_when_the_realm_has_no_sender()
  {
    SendMessagePayload payload = new("PasswordRecovery");
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      Address = Faker.Person.Email,
      DisplayName = Faker.Person.FullName
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<NoDefaultSenderException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
  }

  [Fact(DisplayName = "It should throw SenderNotFoundException when the sender could not be found.")]
  public async Task It_should_throw_SenderNotFoundException_when_the_sender_could_not_be_found()
  {
    SendMessagePayload payload = new("PasswordRecovery")
    {
      SenderId = Guid.NewGuid()
    };
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      Address = Faker.Person.Email,
      DisplayName = Faker.Person.FullName
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SenderNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.SenderId, exception.Id);
    Assert.Equal("SenderId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw SenderNotInTenantException when the sender is in another realm.")]
  public async Task It_should_throw_SenderNotInTenantException_when_the_sender_is_in_another_realm()
  {
    await CreateSenderAsync();
    Assert.NotNull(_sender);

    SetRealm();
    await CreateTemplateAsync(TenantId);

    SendMessagePayload payload = new("PasswordRecovery")
    {
      SenderId = _sender.Id.ToGuid()
    };
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      Address = Faker.Person.Email,
      DisplayName = Faker.Person.FullName
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<SenderNotInTenantException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(_sender.Id, exception.SenderId);
    Assert.Equal(TenantId, exception.ExpectedTenantId);
    Assert.Equal(_sender.TenantId, exception.ActualTenantId);
  }

  [Fact(DisplayName = "It should throw TemplateNotFoundException when the template could not be found.")]
  public async Task It_should_throw_TemplateNotFoundException_when_the_template_could_not_be_found()
  {
    await CreateSenderAsync();

    SendMessagePayload payload = new("PasswordRecovery");
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      Address = Faker.Person.Email,
      DisplayName = Faker.Person.FullName
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TemplateNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Template, exception.Identifier);
    Assert.Equal("Template", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TemplateNotInTenantException when the template is in another realm.")]
  public async Task It_should_throw_TemplateNotInTenantException_when_the_template_is_in_another_realm()
  {
    await CreateSenderAsync();
    Assert.NotNull(_sender);

    await CreateTemplateAsync(TenantId);
    Assert.NotNull(_template);

    SendMessagePayload payload = new(_template.Id.ToGuid().ToString());
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      Address = Faker.Person.Email,
      DisplayName = Faker.Person.FullName
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TemplateNotInTenantException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(_template.Id, exception.TemplateId);
    Assert.Null(exception.ExpectedTenantId);
    Assert.Equal(_template.TenantId, exception.ActualTenantId);
  }

  [Fact(DisplayName = "It should throw UsersNotFoundException when some users could not be found.")]
  public async Task It_should_throw_UsersNotFoundException_when_some_users_could_not_be_found()
  {
    await CreateSenderAsync();

    Guid[] userIds = [Guid.NewGuid(), Guid.NewGuid()];

    SendMessagePayload payload = new("PasswordRecovery");
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      UserId = userIds[0]
    });
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.CC,
      UserId = userIds[1]
    });
    payload.Recipients.Add(new RecipientPayload
    {
      Address = Faker.Person.Email
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UsersNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(userIds, exception.Ids);
    Assert.Equal("Recipients", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UsersNotInTenantException when some users are in another realm.")]
  public async Task It_should_throw_UsersNotInTenantException_when_some_users_are_in_another_realm()
  {
    await CreateSenderAsync(TenantId);
    Assert.NotNull(_sender);

    await CreateTemplateAsync(TenantId);
    Assert.NotNull(_template);

    SetRealm();

    UserId userId = (await _userRepository.LoadAsync()).Single().Id;

    SendMessagePayload payload = new(_template.Id.ToGuid().ToString());
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      UserId = userId.ToGuid()
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UsersNotInTenantException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(userId, exception.UserIds.Single());
    Assert.Equal(TenantId, exception.ExpectedTenantId);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    await CreateSenderAsync();

    SendMessagePayload payload = new("PasswordRecovery");
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("RecipientsValidator", exception.Errors.Single().ErrorCode);
  }

  private async Task<Locale> CreateDictionariesAsync(TenantId? tenantId = null)
  {
    Locale locale = new("fr-CA");
    DictionaryAggregate canadianFrench = new(locale, tenantId);
    canadianFrench.SetEntry("Hello", "Bonjour {name} !");
    canadianFrench.Update();

    DictionaryAggregate french = new(new Locale("fr"), tenantId);
    french.SetEntry("Team", "L'équipe Logitar");
    french.Update();

    Assert.NotNull(Realm.DefaultLocale);
    DictionaryAggregate @default = new(new Locale(Realm.DefaultLocale.Code), tenantId);
    @default.SetEntry("Cordially", "Cordially,");
    @default.SetEntry("PasswordRecovery_ClickLink", "Click on the link below to reset your password.");
    @default.SetEntry("PasswordRecovery_LostYourPassword", "It seems you have lost your password...");
    @default.SetEntry("PasswordRecovery_OneTimePassword", "Your password reset code is {code}. This code is only valid for one hour.");
    @default.SetEntry("PasswordRecovery_Otherwise", "If we've been mistaken, we suggest you to delete this message.");
    @default.SetEntry("PasswordRecovery_Subject", "Reset your password");
    @default.Update();

    await _dictionaryRepository.SaveAsync([canadianFrench, french, @default]);

    return locale;
  }

  private async Task CreateSenderAsync(TenantId? tenantId = null, SenderProvider provider = SenderProvider.SendGrid, bool isDefault = true)
  {
    EmailUnit? email = null;
    PhoneUnit? phone = null;
    Domain.Senders.SenderSettings settings;
    switch (provider)
    {
      case SenderProvider.Mailgun:
        email = new(_senderSettings.Mailgun.Address, isVerified: false);
        settings = new ReadOnlyMailgunSettings(_senderSettings.Mailgun);
        break;
      case SenderProvider.SendGrid:
        email = new(_senderSettings.SendGrid.Address, isVerified: false);
        settings = new ReadOnlySendGridSettings(_senderSettings.SendGrid);
        break;
      case SenderProvider.Twilio:
        phone = new(_senderSettings.Twilio.PhoneNumber, countryCode: null, extension: null, isVerified: false);
        settings = new ReadOnlyTwilioSettings(_senderSettings.Twilio);
        break;
      default:
        throw new SenderProviderNotSupportedException(provider);
    }

    SenderType type = provider.GetSenderType();
    switch (type)
    {
      case SenderType.Email:
        if (email == null)
        {
          throw new InvalidOperationException("The sender email address has not been initialized.");
        }
        _sender = new(email, settings, tenantId)
        {
          DisplayName = DisplayName.TryCreate(_senderSettings.DisplayName)
        };
        _sender.Update();
        break;
      case SenderType.Sms:
        if (phone == null)
        {
          throw new InvalidOperationException("The sender phone number has not been initialized.");
        }
        _sender = new(phone, settings, tenantId);
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }

    if (isDefault)
    {
      _sender.SetDefault();
    }
    await _senderRepository.SaveAsync(_sender);
  }

  private async Task CreateTemplateAsync(TenantId? tenantId = null, SenderProvider provider = SenderProvider.SendGrid)
  {
    ContentUnit content;
    SenderType type = provider.GetSenderType();
    switch (type)
    {
      case SenderType.Email:
        string text = await File.ReadAllTextAsync("Templates/PasswordRecovery.html");
        content = ContentUnit.Html(text);
        break;
      case SenderType.Sms:
        content = ContentUnit.PlainText(@"@(Model.Resource(""PasswordRecovery_OneTimePassword"").Replace(""{code}"", Model.Variable(""Code"")))");
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }

    UniqueKeyUnit uniqueKey = new("PasswordRecovery");
    SubjectUnit subject = new("PasswordRecovery_Subject");
    _template = new TemplateAggregate(uniqueKey, subject, content, tenantId)
    {
      DisplayName = new("Password Recovery")
    };
    await _templateRepository.SaveAsync(_template);
  }
}
