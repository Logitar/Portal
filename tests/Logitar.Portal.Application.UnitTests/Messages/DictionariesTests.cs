using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class DictionariesTests
{
  private readonly Dictionaries _dictionaries;

  public DictionariesTests()
  {
    DictionaryAggregate @default = new(new ReadOnlyLocale("en"));
    @default.SetEntry("ClickLink", "Please click the link below.");
    @default.SetEntry("Message", "Hello World!");
    @default.SetEntry("PasswordRecovery_Subject", "Reset your password");

    DictionaryAggregate fallback = new(new ReadOnlyLocale("fr"));
    fallback.SetEntry("ClickLink", "Veuillez cliquer sur le lien suivant.");
    fallback.SetEntry("PasswordRecovery_Subject", "Réinitialiser votre mot de passe");

    DictionaryAggregate target = new(new ReadOnlyLocale("fr-CA"));
    target.SetEntry("ClickLink", "Veuillez cliquer sur le lien ci-dessous.");

    _dictionaries = new(target, fallback, @default);
  }

  [Fact(DisplayName = "It should return the key when a resource could not be translated.")]
  public void It_should_return_the_key_when_a_resource_could_not_be_translated()
  {
    string key = "CreateUser_Subject";
    Assert.Equal(key, _dictionaries.Translate(key));
  }

  [Fact(DisplayName = "It should translate the resource from the default dictionary.")]
  public void It_should_translate_the_resource_from_the_default_dictionary()
  {
    Assert.Equal("Hello World!", _dictionaries.Translate("Message"));
  }

  [Fact(DisplayName = "It should translate the resource from the fallback dictionary.")]
  public void It_should_translate_the_resource_from_the_fallback_dictionary()
  {
    Assert.Equal("Réinitialiser votre mot de passe", _dictionaries.Translate("PasswordRecovery_Subject"));
  }

  [Fact(DisplayName = "It should translate the resource from the target dictionary.")]
  public void It_should_translate_the_resource_from_the_target_dictionary()
  {
    Assert.Equal("Veuillez cliquer sur le lien ci-dessous.", _dictionaries.Translate("ClickLink"));
  }
}
