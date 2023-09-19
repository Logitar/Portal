using Logitar.EventSourcing;
using Logitar.Portal.Domain.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys;

internal record XApiKey
{
  private const string Prefix = "PT";
  private const char Separator = '.';

  public XApiKey(ApiKeyAggregate apiKey, byte[] secret) : this(apiKey.Id, secret)
  {
  }

  private XApiKey(AggregateId id, byte[] secret)
  {
    Id = id;
    Secret = secret;
  }

  public AggregateId Id { get; }
  public byte[] Secret { get; }

  public static XApiKey Decode(string value)
  {
    string[] parts = value.Split(Separator);
    if (parts.Length != 3 || parts.First() != Prefix)
    {
      throw new ArgumentException($"The X-API-Key '{value}' is not valid.");
    }

    return new XApiKey(new AggregateId(new Guid(Convert.FromBase64String(parts[1].FromUriSafeBase64()))),
      Convert.FromBase64String(parts[2].FromUriSafeBase64()));
  }

  public string Encode() => string.Join(Separator, Prefix,
    Convert.ToBase64String(Id.ToGuid().ToByteArray()).ToUriSafeBase64(),
    Convert.ToBase64String(Secret).ToUriSafeBase64());
}
