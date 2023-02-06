using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Users;
using System.Security.Claims;

namespace Logitar.Portal.Application.Claims
{
  public static class ClaimExtensions
  {
    public static ClaimsIdentity GetClaimsIdentity(this ApiKeyModel apiKey, string? authenticationScheme = null)
    {
      ClaimsIdentity identity = new(authenticationScheme);

      identity.AddClaim(new(Rfc7519ClaimTypes.Subject, apiKey.Id.ToString()));
      identity.AddClaim((apiKey.UpdatedOn ?? apiKey.CreatedOn).GetClaim(Rfc7519ClaimTypes.UpdatedOn));
      identity.AddClaim(new(Rfc7519ClaimTypes.FullName, apiKey.Title));
      identity.AddClaim(DateTime.UtcNow.GetClaim(Rfc7519ClaimTypes.AuthenticationTime));

      if (apiKey.ExpiresOn.HasValue)
      {
        identity.AddClaim(apiKey.ExpiresOn.Value.GetClaim(Rfc7519ClaimTypes.Expires));
      }

      return identity;
    }
    public static ClaimsIdentity GetClaimsIdentity(this UserModel user, string? authenticationScheme = null)
    {
      ClaimsIdentity identity = new(authenticationScheme);

      identity.AddClaim(new(Rfc7519ClaimTypes.Subject, user.Id.ToString()));
      identity.AddClaim(new(Rfc7519ClaimTypes.Username, user.Username));
      identity.AddClaim((user.UpdatedOn ?? user.CreatedOn).GetClaim(Rfc7519ClaimTypes.UpdatedOn));

      if (user.Email != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.Email, user.Email));
        identity.AddClaim(new(Rfc7519ClaimTypes.EmailVerified, user.EmailConfirmedOn.HasValue.ToString().ToLower(), ClaimValueTypes.Boolean));
      }
      if (user.PhoneNumber != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.PhoneNumber, user.PhoneNumber));
        identity.AddClaim(new(Rfc7519ClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmedOn.HasValue.ToString().ToLower(), ClaimValueTypes.Boolean));
      }

      if (user.FullName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.FullName, user.FullName));

        if (user.FirstName != null)
        {
          identity.AddClaim(new(Rfc7519ClaimTypes.FirstName, user.FirstName));
        }

        if (user.MiddleName != null)
        {
          identity.AddClaim(new(Rfc7519ClaimTypes.MiddleName, user.MiddleName));
        }

        if (user.LastName != null)
        {
          identity.AddClaim(new(Rfc7519ClaimTypes.LastName, user.LastName));
        }
      }

      if (user.Locale != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.Locale, user.Locale));
      }

      if (user.Picture != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.PictureUri, user.Picture));
      }

      if (user.SignedInOn.HasValue)
      {
        identity.AddClaim(user.SignedInOn.Value.GetClaim(Rfc7519ClaimTypes.AuthenticationTime));
      }

      return identity;
    }
    private static Claim GetClaim(this DateTime moment, string type)
      => new(type, new DateTimeOffset(moment).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);

    public static DateTime GetDateTime(this Claim claim)
      => DateTimeOffset.FromUnixTimeSeconds(long.Parse(claim.Value)).UtcDateTime;
  }
}
