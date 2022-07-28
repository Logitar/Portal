using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Portal.Infrastructure.Tokens
{
  internal class InvalidSecurityTokenPurposeException : SecurityTokenValidationException
  {
    public InvalidSecurityTokenPurposeException(string requiredPurpose, IEnumerable<string> actualPurposes)
      : base(GetMessage(requiredPurpose, actualPurposes))
    {
      ActualPurposes = actualPurposes ?? throw new ArgumentNullException(nameof(actualPurposes));
      RequiredPurpose = requiredPurpose ?? throw new ArgumentNullException(nameof(requiredPurpose));
    }

    public IEnumerable<string> ActualPurposes { get; }
    public string RequiredPurpose { get; }

    private static string GetMessage(string requiredPurpose, IEnumerable<string> actualPurposes)
    {
      var message = new StringBuilder();

      message.AppendLine("The security token does not serve the required purpose.");
      message.AppendLine($"Required purpose: {requiredPurpose}");
      message.AppendLine($"Actual purposes: {string.Join(", ", actualPurposes ?? Enumerable.Empty<string>())}");

      return message.ToString();
    }
  }
}
