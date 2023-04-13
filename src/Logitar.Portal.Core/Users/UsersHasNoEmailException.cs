using System.Text;

namespace Logitar.Portal.Core.Users;

public class UsersHasNoEmailException : Exception
{
  public UsersHasNoEmailException(IEnumerable<Guid> ids, string paramName)
    : base(GetMessage(ids, paramName))
  {
    Data[nameof(Ids)] = ids;
    Data[nameof(ParamName)] = paramName;
  }

  public IEnumerable<Guid> Ids => (IEnumerable<Guid>)Data[nameof(Ids)]!;
  public string ParamName => (string)Data[nameof(ParamName)]!;

  private static string GetMessage(IEnumerable<Guid> ids, string paramName)
  {
    StringBuilder message = new();

    message.AppendLine("The specified users do not have an email address.");
    message.Append("ParamName: ").AppendLine(paramName);

    message.AppendLine();
    message.AppendLine("Ids:");
    foreach (Guid id in ids)
    {
      message.Append(" - ").Append(id).AppendLine();
    }

    return message.ToString();
  }
}
