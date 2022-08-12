using Logitar.Portal.Core;
using Logitar.Portal.Core.Actors;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  public class Log
  {
    public Log(string method, string url, string? ipAddress = null)
    {
      Id = Guid.NewGuid();

      Method = method ?? throw new ArgumentNullException(nameof(method));
      Url = url ?? throw new ArgumentNullException(nameof(url));
      IpAddress = ipAddress;

      StartedAt = DateTime.UtcNow;
    }
    private Log()
    {
    }

    public long Sid { get; private set; }
    public Guid Id { get; private set; }

    public string Method { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public string? IpAddress { get; private set; }
    public int? StatusCode { get; private set; }

    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public TimeSpan? Duration
    {
      get => EndedAt.HasValue ? EndedAt.Value - StartedAt : null;
      private set { /* EntityFrameworkCore only setter */ }
    }

    public Guid ActorId { get; private set; }
    public Guid? SessionId { get; private set; }
    public Guid? UserId { get; private set; }

    public List<Error> Errors { get; private set; } = new();
    public string? ErrorsSerialized
    {
      get => Errors.Any() ? JsonSerializer.Serialize(Errors) : null;
      private set { /* EntityFrameworkCore only setter */ }
    }
    public bool HasErrors
    {
      get => Errors.Any();
      private set { /* EntityFrameworkCore only setter */ }
    }
    public bool Succeeded
    {
      get => StatusCode.HasValue && !HasErrors;
      private set { /* EntityFrameworkCore only setter */ }
    }

    public Dictionary<string, object?> Request { get; private set; } = new();
    public string? RequestSerialized
    {
      get => Request.Any() ? JsonSerializer.Serialize(Request) : null;
      private set { /* EntityFrameworkCore only setter */ }
    }

    public void Complete(int statusCode, Actor actor, Session? session = null, User? user = null)
    {
      ArgumentNullException.ThrowIfNull(actor);

      StatusCode = statusCode;

      EndedAt = DateTime.UtcNow;

      ActorId = actor.Id;
      SessionId = session?.Id;
      UserId = user?.Id;
    }
  }
}
