namespace Logitar.Portal.Domain;

[Trait(Traits.Category, Categories.Unit)]
public class LocaleTests
{
  [Fact(DisplayName = "It should be the correct default locale.")]
  public void It_should_be_the_correct_default_locale()
  {
    Locale locale = Locale.Default;
    Assert.Equal("en", locale.Code);
  }

  [Fact(DisplayName = "It should have a parent locale when it is a regional locale.")]
  public void It_should_have_a_parent_locale_when_it_is_a_regional_locale()
  {
    Locale locale = new("fr-CA");
    Locale parent = new("fr");
    Assert.Equal(parent, locale.Parent);
  }

  [Fact(DisplayName = "It should have the correct locale code.")]
  public void It_should_have_the_correct_locale_code()
  {
    string code = "  fr-CA  ";

    Locale locale = new(code);
    Assert.Equal(code.Trim(), locale.Code);
  }

  [Fact(DisplayName = "It should not have a parent locale when it is a language locale.")]
  public void It_should_not_have_a_parent_locale_when_it_is_a_language_locale()
  {
    Locale locale = new("fr");
    Assert.Null(locale.Parent);
  }

  [Fact(DisplayName = "It should throw ArgumentOutOfRangeException when the locale is an user-defined culture.")]
  public void It_should_throw_ArgumentOutOfRangeException_when_the_locale_is_an_user_defined_culture()
  {
    var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Locale("fr-US"));
    Assert.Equal("code", exception.ParamName);
  }

  [Theory(DisplayName = "It should throw ArgumentException when the code is null or white space.")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("  ")]
  public void It_should_throw_ArgumentException_when_the_code_is_null_or_white_space(string code)
  {
    var exception = Assert.Throws<ArgumentException>(() => new Locale(code));
    Assert.Equal("code", exception.ParamName);
  }
}
