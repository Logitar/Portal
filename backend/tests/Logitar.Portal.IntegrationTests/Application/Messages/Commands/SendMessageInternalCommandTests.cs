using FluentValidation;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Messages.Settings;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Application.Templates;
using Logitar.Portal.Application.Tokens.Commands;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;
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

  private readonly EmailSettings _recipientSettings;
  private readonly SenderSettings _senderSettings;

  private SenderAggregate? _sender = null;
  private TemplateAggregate? _template = null;

  public SendMessageInternalCommandTests() : base()
  {
    _dictionaryRepository = ServiceProvider.GetRequiredService<IDictionaryRepository>();
    _messageRepository = ServiceProvider.GetRequiredService<IMessageRepository>();
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

    _recipientSettings = ServiceProvider.GetRequiredService<IConfiguration>().GetSection("Recipient").Get<EmailSettings>() ?? new();
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
  private async Task It_should_send_a_message_with_a_provider(SenderProvider provider, bool ignoreUserLocale = true)
  {
    Assert.NotNull(Realm.DefaultLocale);
    SetRealm();

    LocaleUnit locale = await CreateDictionariesAsync(TenantId);

    await CreateSenderAsync(TenantId, provider);
    Assert.NotNull(_sender);

    await CreateTemplateAsync(TenantId);
    Assert.NotNull(_template);

    UniqueNameUnit uniqueName = new(Realm.UniqueNameSettings, _recipientSettings.Address);
    UserAggregate user = new(uniqueName, TenantId);
    user.SetEmail(new EmailUnit(_recipientSettings.Address, isVerified: false));
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
    user.Locale = ignoreUserLocale ? new LocaleUnit(Realm.DefaultLocale.Code) : locale;
    user.Update();
    await _userRepository.SaveAsync(user);

    CreateTokenPayload createToken = new()
    {
      IsConsumable = true,
      Type = "reset_password+jwt",
      Subject = user.Id.ToGuid().ToString()
    };
    CreatedToken createdToken = await ActivityPipeline.ExecuteAsync(new CreateTokenCommand(createToken));

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
    payload.Variables.Add(new Variable("Token", createdToken.Token));
    SendMessageInternalCommand command = new(payload);
    SentMessages sentMessages = await ActivityPipeline.ExecuteAsync(command);

    Guid id = Assert.Single(sentMessages.Ids);
    MessageAggregate? message = await _messageRepository.LoadAsync(id);
    Assert.NotNull(message);

    Assert.Equal(TenantId, message.TenantId);
    Assert.Equal("Reset your password", message.Subject.Value);
    Assert.Equal(MediaTypeNames.Text.Html, message.Body.Type);
    Assert.DoesNotContain("Model.", message.Body.Text);
    Assert.Contains($"Bonjour {user.FullName} !", message.Body.Text);
    Assert.Contains("L&#39;&#233;quipe Logitar", message.Body.Text);
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
    Assert.Equal(user.Id, recipient.UserId);

    KeyValuePair<string, string> variable = Assert.Single(message.Variables);
    Assert.Equal("Token", variable.Key);
    Assert.Equal(createdToken.Token, variable.Value);
  }

  [Fact(DisplayName = "It should throw MissingRecipientAddressesException when an user is missing an email.")]
  public async Task It_should_throw_MissingRecipientAddressesException_when_an_user_is_missing_an_email()
  {
    UserAggregate user = new(new UniqueNameUnit(Realm.UniqueNameSettings, UsernameString), TenantId);
    await _userRepository.SaveAsync(user);

    SetRealm();

    SendMessagePayload payload = new("PasswordRecovery");
    payload.Recipients.Add(new RecipientPayload
    {
      Type = RecipientType.To,
      UserId = user.Id.ToGuid()
    });
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<MissingRecipientAddressesException>(async () => await ActivityPipeline.ExecuteAsync(command));
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
    SendMessagePayload payload = new("PasswordRecovery");
    SendMessageInternalCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("RecipientsValidator", exception.Errors.Single().ErrorCode);
  }

  private async Task<LocaleUnit> CreateDictionariesAsync(TenantId? tenantId = null)
  {
    LocaleUnit locale = new("fr-CA");
    DictionaryAggregate canadianFrench = new(locale, tenantId);
    canadianFrench.SetEntry("Hello", "Bonjour {name} !");
    canadianFrench.Update();

    DictionaryAggregate french = new(new LocaleUnit("fr"), tenantId);
    french.SetEntry("Team", "L'équipe Logitar");
    french.Update();

    Assert.NotNull(Realm.DefaultLocale);
    DictionaryAggregate @default = new(new LocaleUnit(Realm.DefaultLocale.Code), tenantId);
    @default.SetEntry("Cordially", "Cordially,");
    @default.SetEntry("PasswordRecovery_ClickLink", "Click on the link below to reset your password.");
    @default.SetEntry("PasswordRecovery_LostYourPassword", "It seems you have lost your password...");
    @default.SetEntry("PasswordRecovery_Otherwise", "If we've been mistaken, we suggest you to delete this message.");
    @default.SetEntry("PasswordRecovery_Subject", "Reset your password");
    @default.Update();

    await _dictionaryRepository.SaveAsync([canadianFrench, french, @default]);

    return locale;
  }

  private async Task CreateSenderAsync(TenantId? tenantId = null, SenderProvider provider = SenderProvider.SendGrid, bool isDefault = true)
  {
    EmailUnit email;
    Domain.Senders.SenderSettings settings;
    switch (provider)
    {
      case SenderProvider.Mailgun:
        email = new(_senderSettings.Mailgun.Address, isVerified: false);
        settings = new ReadOnlyMailgunSettings(_senderSettings.Mailgun.ApiKey, _senderSettings.Mailgun.DomainName);
        break;
      case SenderProvider.SendGrid:
        email = new(_senderSettings.SendGrid.Address, isVerified: false);
        settings = new ReadOnlySendGridSettings(_senderSettings.SendGrid.ApiKey);
        break;
      default:
        throw new SenderProviderNotSupportedException(provider);
    }

    _sender = new SenderAggregate(email, settings, tenantId)
    {
      DisplayName = DisplayNameUnit.TryCreate(_senderSettings.DisplayName)
    };
    _sender.Update();
    if (isDefault)
    {
      _sender.SetDefault();
    }
    await _senderRepository.SaveAsync(_sender);
  }

  private async Task CreateTemplateAsync(TenantId? tenantId = null)
  {
    string text = await File.ReadAllTextAsync("Templates/PasswordRecovery.html");
    UniqueKeyUnit uniqueKey = new("PasswordRecovery");
    SubjectUnit subject = new("PasswordRecovery_Subject");
    ContentUnit content = ContentUnit.Html(text);
    _template = new TemplateAggregate(uniqueKey, subject, content, tenantId)
    {
      DisplayName = new("Password Recovery")
    };
    await _templateRepository.SaveAsync(_template);
  }
}
