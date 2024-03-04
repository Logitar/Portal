using Bogus;
using Logitar.Portal.Application;

namespace Logitar.Portal;

internal class TestBaseUrl : IBaseUrl
{
  private readonly Faker _faker = new();

  public string Value { get; }

  public TestBaseUrl()
  {
    Value = $"https://www.{_faker.Internet.DomainName()}";
  }
}
