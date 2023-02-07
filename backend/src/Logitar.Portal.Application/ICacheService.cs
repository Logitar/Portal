using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application
{
  public interface ICacheService
  {
    Configuration? Configuration { get; set; }
  }
}
