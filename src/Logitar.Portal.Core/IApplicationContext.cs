namespace Logitar.Portal.Core;

public interface IApplicationContext
{
  Guid? ActivityId { get; set; }

  Uri? BaseUrl { get; }
}
