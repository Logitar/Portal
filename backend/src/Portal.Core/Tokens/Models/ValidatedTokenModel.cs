using Portal.Core.Claims;
using System.Security.Claims;

namespace Portal.Core.Tokens.Models
{
  public class ValidatedTokenModel
  {
    public ValidatedTokenModel(ValidateTokenResult result)
    {
      ArgumentNullException.ThrowIfNull(result);

      if (result.Principal == null)
      {
        Errors = result.Errors;
      }
      else
      {
        Errors = Enumerable.Empty<Error>();

        var claims = new List<ClaimModel>(capacity: result.Principal.Claims.Count());

        foreach (Claim claim in result.Principal.Claims)
        {
          switch (claim.Type)
          {
            case Rfc7519ClaimTypes.Email:
              Email = claim.Value;
              break;
            case Rfc7519ClaimTypes.Subject:
              Subject = claim.Value;
              break;
            default:
              claims.Add(new ClaimModel(claim));
              break;
          }
        }

        Claims = claims;
      }
    }

    public IEnumerable<Error> Errors { get; }
    public bool Succeeded => !Errors.Any();

    public string? Email { get; }
    public string? Subject { get; }

    public IEnumerable<ClaimModel>? Claims { get; }
  }
}
