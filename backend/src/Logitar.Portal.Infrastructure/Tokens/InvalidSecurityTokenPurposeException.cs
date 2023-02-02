using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Logitar.Portal.Infrastructure.Tokens
{
  internal class InvalidSecurityTokenPurposeException : SecurityTokenValidationException
  {
    public InvalidSecurityTokenPurposeException(string requiredPurpose, IEnumerable<string> actualPurposes)
      : base(GetMessage(requiredPurpose, actualPurposes))
    {
      Data["RequiredPurpose"] = requiredPurpose;
      Data["ActualPurposes"] = actualPurposes;
    }

    private static string GetMessage(string requiredPurpose, IEnumerable<string> actualPurposes)
    {
      StringBuilder message = new();

      message.AppendLine("The security token does not serve the required purpose.");
      message.AppendLine($"Required purpose: {requiredPurpose}");
      message.AppendLine($"Actual purposes: {string.Join(", ", actualPurposes ?? Enumerable.Empty<string>())}");

      return message.ToString();
    }
  }
}
