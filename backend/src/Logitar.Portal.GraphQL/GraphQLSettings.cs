namespace Logitar.Portal.GraphQL;

public record GraphQLSettings
{
  public bool EnableMetrics { get; set; }
  public bool ExposeExceptionDetails { get; set; }
}
