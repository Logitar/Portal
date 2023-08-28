using Logitar.EventSourcing;
using Logitar.Portal.Domain.Sessions;

namespace Logitar.Portal.Application.Sessions;

internal record RefreshToken
{
  private const string Prefix = "RT";
  private const char Separator = '.';

  public RefreshToken(SessionAggregate session, byte[] secret) : this(session.Id, secret)
  {
  }

  private RefreshToken(AggregateId id, byte[] secret)
  {
    Id = id;
    Secret = secret;
  }

  public AggregateId Id { get; }
  public byte[] Secret { get; }

  public static RefreshToken Decode(string value)
  {
    string[] parts = value.Split(Separator);
    if (parts.Length != 3 || parts.First() != Prefix)
    {
      throw new ArgumentException($"The refresh token '{value}' is not valid.");
    }

    return new RefreshToken(new AggregateId(new Guid(Convert.FromBase64String(parts[1].FromUriSafeBase64()))),
      Convert.FromBase64String(parts[2].FromUriSafeBase64()));
  }

  public string Encode() => string.Join(Separator, Prefix,
    Convert.ToBase64String(Id.ToGuid().ToByteArray()).ToUriSafeBase64(),
    Convert.ToBase64String(Secret).ToUriSafeBase64());
}
