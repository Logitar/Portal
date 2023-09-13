using Bogus;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Domain.Realms;

[Trait(Traits.Category, Categories.Unit)]
public class RealmAggregateTests
{
  private readonly Faker _faker = new();

  private readonly RealmAggregate _realm;

  public RealmAggregateTests()
  {
    _realm = new("desjardins", requireUniqueEmail: true, requireConfirmedAccount: true)
    {
      DisplayName = "Desjardins",
      DefaultLocale = new Locale(_faker.Locale),
      Url = new Uri("https://www.desjardins.com/")
    };
  }

  [Fact(DisplayName = "It should remove the password recovery sender.")]
  public void It_should_remove_the_password_recovery_sender()
  {
    SenderAggregate sender = new(_faker.Internet.Email(), ProviderType.SendGrid, isDefault: true, tenantId: _realm.Id.Value)
    {
      DisplayName = _faker.Name.FullName()
    };
    _realm.SetPasswordRecoverySender(sender, nameof(sender));
    Assert.Equal(sender.Id, _realm.PasswordRecoverySenderId);

    _realm.RemovePasswordRecoverySender();
    Assert.Null(_realm.PasswordRecoverySenderId);
  }

  [Fact(DisplayName = "It should set the specified password recovery sender.")]
  public void It_should_set_the_specified_password_recovery_sender()
  {
    SenderAggregate sender = new(_faker.Internet.Email(), ProviderType.SendGrid, isDefault: true, tenantId: _realm.Id.Value)
    {
      DisplayName = _faker.Name.FullName()
    };

    _realm.SetPasswordRecoverySender(sender, nameof(sender));
    Assert.Equal(sender.Id, _realm.PasswordRecoverySenderId);
  }

  [Fact(DisplayName = "It should throw SenderNotInRealmException when setting a password recovery sender from a different realm.")]
  public void It_should_throw_SenderNotInRealmException_when_setting_a_password_recovery_sender_from_a_different_realm()
  {
    SenderAggregate sender = new(_faker.Internet.Email(), ProviderType.SendGrid, isDefault: true, tenantId: null);

    var exception = Assert.Throws<SenderNotInRealmException>(() => _realm.SetPasswordRecoverySender(sender, nameof(sender)));
    Assert.Equal(sender.ToString(), exception.Sender);
    Assert.Equal(_realm.ToString(), exception.Realm);
    Assert.Equal(nameof(sender), exception.PropertyName);
  }
}
