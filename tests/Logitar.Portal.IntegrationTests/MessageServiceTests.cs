using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class MessageServiceTests : IntegrationTests, IAsyncLifetime
{
  private const string SenderKey = "Sender";
  private const string SendGridKey = "SendGrid";

  private readonly IMessageService _messageService;

  private readonly RealmAggregate _realm;
  private readonly SenderAggregate _sender;
  private readonly TemplateAggregate _template;
  private readonly UserAggregate _user;

  public MessageServiceTests()
  {
    _messageService = ServiceProvider.GetRequiredService<IMessageService>();

    _realm = new("logitar")
    {
      DefaultLocale = new Locale("fr"),
      DisplayName = "Logitar"
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
      Email = new EmailAddress(Faker.Person.Email),
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = new Gender(Faker.Person.Gender.ToString()),
      Locale = _realm.DefaultLocale,
      Picture = new Uri(Faker.Person.Avatar),
      Website = new Uri($"https://www.{Faker.Person.Website}/")
    };
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _sender, _template, _user });
  }

  [Fact(DisplayName = "It should send a message to a Portal user.")]
  public async Task It_should_send_a_message_to_a_Portal_user()
  {
    Assert.Fail("TODO(fpion): implement");
  }

  [Fact(DisplayName = "It should send a message to a realm user.")]
  public async Task It_should_send_a_message_to_a_realm_user()
  {
    Assert.Fail("TODO(fpion): implement");

    //SendMessagePayload payload = new()
    //{
    //  Realm = $"  {_realm.UniqueSlug.ToUpper()}  ",
    //  SenderId = _sender.Id.ToGuid(),
    //  Template = $"  {_template.UniqueName.ToUpper()}  ",
    //  Recipients = new RecipientPayload[]
    //  {
    //    new()
    //    {
    //      User = $"  {_user.UniqueName.ToUpper()}  "
    //    }
    //  },
    //  IgnoreUserLocale = true,
    //  Locale = $"  {_dictionary.Locale}  ",
    //  Variables = new Variable[]
    //  {
    //    new("Code", "112656"),
    //    new("Guid", Guid.NewGuid().ToString())
    //  }
    //};

    //SentMessages sentMessages = await _messageService.SendAsync(payload);

    // TODO(fpion): implement
  }

  [Fact(DisplayName = "It should send a message to a recipient who is not an user.")]
  public async Task It_should_send_a_message_to_a_recipient_who_is_not_an_user()
  {
    Assert.Fail("TODO(fpion): implement");
  }

  [Fact(DisplayName = "It should send a message to multiple recipients.")]
  public async Task It_should_send_a_message_to_multiple_recipients()
  {
    Assert.Fail("TODO(fpion): implement");
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
      DefaultLocale = new Locale("fr-CA"),
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
    SendMessagePayload payload = new()
    {
      SenderId = _sender.Id.ToGuid()
    };

    var exception = await Assert.ThrowsAsync<SenderNotInRealmException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(_sender.ToString(), exception.Sender);
    Assert.Null(exception.Realm);
    Assert.Equal(nameof(payload.SenderId), exception.PropertyName);
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
      Template = template.Id.ToGuid().ToString()
    };

    var exception = await Assert.ThrowsAsync<TemplateNotInRealmException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(template.ToString(), exception.Template);
    Assert.Equal(_realm.ToString(), exception.Realm);
    Assert.Equal(nameof(payload.Template), exception.PropertyName);
  }

  [Fact(DisplayName = "SendAsync: it should throw UsersNotFoundException when some users are not in the realm.")]
  public async Task SendAsync_it_should_throw_UsersNotFoundException_when_some_users_are_not_in_the_realm()
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
          User = _user.Id.ToGuid().ToString()
        },
        new()
        {
          User = User.Id.ToGuid().ToString()
        },
        new()
        {
          User = Guid.Empty.ToString()
        }
      }
    };

    var exception = await Assert.ThrowsAsync<UsersNotFoundException>(async () => await _messageService.SendAsync(payload));
    Assert.Equal(payload.Recipients.Skip(1).Select(recipient => recipient.User), exception.MissingUsers);
    Assert.Equal(nameof(payload.Recipients), exception.PropertyName);
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
}
