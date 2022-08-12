using System.Security.Claims;

namespace Logitar.Portal.Core.Tokens
{
  public class ValidateTokenResult
  {
    private readonly List<Error> _errors = new();

    private ValidateTokenResult(ClaimsPrincipal? principal = null)
    {
      Principal = principal;
    }

    public IReadOnlyCollection<Error> Errors => _errors.AsReadOnly();
    public ClaimsPrincipal? Principal { get; }
    public bool Succeeded => !Errors.Any() && Principal != null;

    public static ValidateTokenResult Failed(Error error)
    {
      var result = new ValidateTokenResult();
      result.AddError(error);

      return result;
    }
    public static ValidateTokenResult Success(ClaimsPrincipal principal) => new(principal);

    private void AddError(Error error)
    {
      ArgumentNullException.ThrowIfNull(error);

      _errors.Add(error);
    }
  }
}
