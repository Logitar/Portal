namespace Logitar.Portal.Domain.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class SendMessageResultTests
{
  [Fact(DisplayName = "It should return an empty dictionary.")]
  public void It_should_return_an_empty_dictionary()
  {
    SendMessageResult variables = new();
    Assert.Empty(variables.AsDictionary());
  }

  [Fact(DisplayName = "It should return an empty dictionary from a capacity.")]
  public void It_should_return_an_empty_dictionary_from_a_capacity()
  {
    SendMessageResult variables = new(capacity: 3);
    Assert.Empty(variables.AsDictionary());
  }

  [Fact(DisplayName = "It should return the correct dictionary of variables.")]
  public void It_should_return_the_correct_dictionary_of_variables()
  {
    SendMessageResult result = new(new KeyValuePair<string, string>[]
    {
      new("Content", "Hello World!"),
      new("StatusCode", "200"),
      new("StatusCode", "201"),
      new("StatusCode", "202"),
      new("StatusText", "OK"),
      new("StatusText", "Created"),
      new("StatusText", "Accepted"),
      new("Version", "1.1")
    });

    Dictionary<string, string> dictionary = result.AsDictionary();
    Assert.Equal(4, dictionary.Count);
    Assert.Equal("Hello World!", dictionary["Content"]);
    Assert.Equal("202", dictionary["StatusCode"]);
    Assert.Equal("Accepted", dictionary["StatusText"]);
    Assert.Equal("1.1", dictionary["Version"]);
  }
}
