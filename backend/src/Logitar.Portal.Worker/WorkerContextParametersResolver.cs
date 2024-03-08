using Logitar.Portal.Application.Activities;

namespace Logitar.Portal.Worker;

internal class WorkerContextParametersResolver : IContextParametersResolver
{
  public IContextParameters Resolve() => new ContextParameters();
}
