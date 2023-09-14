using Bogus;

namespace Logitar.Portal.Domain.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class VariablesTests
{
  private readonly Faker _faker = new();

  [Fact(DisplayName = "It should populate the correct list of variables.")]
  public void It_should_populate_the_correct_list_of_variables()
  {
    var inputs = new KeyValuePair<string, string>[]
    {
      new("AuthCode", "123456"),
      new("AuthCode", "460495"),
      new("FullName", _faker.Person.FullName)
    };

    Variables variables = new(inputs);

    foreach (KeyValuePair<string, string> variable in inputs.Skip(1))
    {
      Assert.Equal(variable.Value.Trim(), variables.Resolve(variable.Key.Trim()));
    }
  }

  [Fact(DisplayName = "It should return the key when the variable is not resolved.")]
  public void It_should_return_the_key_when_the_variable_is_not_resolved()
  {
    Variables variables = new();

    string key = "AuthCode";
    Assert.Equal(key, variables.Resolve(key));
  }
}
