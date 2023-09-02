using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal.Application.Tokens;

public class InvalidSecurityTokenPurposeException : SecurityTokenValidationException
{
  private const string ErrorMessage = "The security token does not serve the required purpose.";

  public InvalidSecurityTokenPurposeException(string requiredPurpose, IEnumerable<string> actualPurposes)
    : base(BuildMessage(requiredPurpose, actualPurposes))
  {
    RequiredPurpose = requiredPurpose;
    ActualPurposes = actualPurposes;
  }

  public string RequiredPurpose
  {
    get => (string)Data[nameof(RequiredPurpose)]!;
    private set => Data[nameof(RequiredPurpose)] = value;
  }
  public IEnumerable<string> ActualPurposes
  {
    get => (IEnumerable<string>)Data[nameof(ActualPurposes)]!;
    private set => Data[nameof(ActualPurposes)] = value;
  }

  private static string BuildMessage(string requiredPurpose, IEnumerable<string> actualPurposes)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("RequiredPurpose: ").AppendLine(requiredPurpose);

    message.AppendLine("Actual purposes:");
    foreach (string actualPurpose in actualPurposes)
    {
      message.Append(" - ").AppendLine(actualPurpose);
    }

    return message.ToString();
  }
}
