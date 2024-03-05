using Logitar.Portal.Application;

namespace Logitar.Portal.Worker;

internal class WorkerBaseUrl : IBaseUrl
{
  public string Value { get; }

  public WorkerBaseUrl(IConfiguration configuration)
  {
    Value = configuration.GetValue<string>("BaseUrl") ?? throw new InvalidOperationException("The configuration 'BaseUrl' is required.");
  }
}
