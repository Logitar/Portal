using System.Net;
using System.Text;

namespace Portal.Core
{
  public class EntityNotFoundException<T> : ApiException where T : Aggregate
  {
    public EntityNotFoundException(Guid id, string? paramName = null)
      : this(id.ToString(), paramName)
    { 
    }
    public EntityNotFoundException(string id, string? paramName = null)
      : base(HttpStatusCode.NotFound, GetMessage(id, paramName))
    {
      Id = id ?? throw new ArgumentNullException(nameof(id));
      ParamName = paramName;
    }

    public string Id { get; }
    public string? ParamName { get; }

    private static string GetMessage(string id, string? paramName)
    {
      var message = new StringBuilder();

      message.AppendLine("The specified entity could not be found.");
      message.AppendLine($"Type: {typeof(T)}");
      message.AppendLine($"Id: {id}");

      if (paramName != null)
        message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
