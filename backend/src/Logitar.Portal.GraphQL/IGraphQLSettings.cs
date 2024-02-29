namespace Logitar.Portal.GraphQL;

public interface IGraphQLSettings
{
  bool EnableMetrics { get; }
  bool ExposeExceptionDetails { get; }
}
