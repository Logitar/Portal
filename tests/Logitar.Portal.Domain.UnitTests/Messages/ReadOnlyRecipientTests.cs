using Bogus;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class ReadOnlyRecipientTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "It should construct the correct recipient.")]
  public void It_should_construct_the_correct_recipient()
  {
    ReadOnlyRecipient recipient = new($"  {_faker.Person.Email}  ", $"  {_faker.Person.FullName}  ", RecipientType.CC);
    Assert.Equal(_faker.Person.Email, recipient.Address);
    Assert.Equal(_faker.Person.FullName, recipient.DisplayName);
    Assert.Equal(RecipientType.CC, recipient.Type);
  }

  [Fact(DisplayName = "It should construct the correct recipient from an user.")]
  public void It_should_construct_the_correct_recipient_from_an_user()
  {
    RealmAggregate realm = new("logitar");
    UserAggregate user = new(realm.UniqueNameSettings, _faker.Person.UserName, realm.Id.Value)
    {
      Email = new EmailAddress(_faker.Person.Email),
      FirstName = _faker.Person.FirstName,
      LastName = _faker.Person.LastName
    };

    ReadOnlyRecipient recipient = ReadOnlyRecipient.From(user);
    Assert.Equal(RecipientType.To, recipient.Type);
    Assert.Equal(user.Email.Address, recipient.Address);
    Assert.Equal(user.FullName, recipient.DisplayName);
    Assert.Equal(user.Id, recipient.UserId);
  }

  [Fact(DisplayName = "It should throw ArgumentException when constructing from an user without an email.")]
  public void It_should_throw_ArgumentException_when_constructing_from_an_user_without_an_email()
  {
    RealmAggregate realm = new("logitar");
    UserAggregate user = new(realm.UniqueNameSettings, _faker.Person.UserName, realm.Id.Value);

    var exception = Assert.Throws<ArgumentException>(() => ReadOnlyRecipient.From(user));
    Assert.Equal("user", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw ValidationException when constructing an invalid recipient.")]
  public void It_should_throw_ValidationException_when_constructing_an_invalid_recipient()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new ReadOnlyRecipient("aa@@bb..cc", type: (RecipientType)(-1)));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EmailValidator" && e.PropertyName == "Address");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Type");
  }
}
