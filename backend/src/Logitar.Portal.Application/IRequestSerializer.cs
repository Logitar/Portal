using MediatR;

namespace Logitar.Portal.Application
{
  public interface IRequestSerializer
  {
    string Serialize<T>(IRequest<T> request);
  }
}
