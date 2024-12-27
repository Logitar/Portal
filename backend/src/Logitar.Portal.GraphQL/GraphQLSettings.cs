namespace Logitar.Portal.GraphQL;

public record GraphQLSettings : IGraphQLSettings
{
  public const string SectionKey = "GraphQL";

  public bool EnableMetrics { get; set; }
  public bool ExposeExceptionDetails { get; set; }
}
