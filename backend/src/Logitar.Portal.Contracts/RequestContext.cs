namespace Logitar.Portal.Contracts;

public record RequestContext : IRequestContext
{
  public string? User { get; set; }

  public CancellationToken CancellationToken { get; set; }

  public RequestContext() : this(cancellationToken: default)
  {
  }

  public RequestContext(CancellationToken cancellationToken) : this(user: null, cancellationToken)
  {
  }

  public RequestContext(string? user, CancellationToken cancellationToken = default)
  {
    User = user;
    CancellationToken = cancellationToken;
  }
}
