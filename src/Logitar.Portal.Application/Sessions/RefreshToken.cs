using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;

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
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values.First() != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid refresh token.", nameof(value));
    }

    return new RefreshToken(new AggregateId(values[1]), Convert.FromBase64String(values[2].FromUriSafeBase64()));
  }

  public string Encode() => string.Join(Separator, Prefix, Id.Value, Convert.ToBase64String(Secret).ToUriSafeBase64());
}
