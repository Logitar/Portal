using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.Users;
using System.Security.Claims;

namespace Logitar.Portal.Core.Claims
{
  public static class ClaimExtensions
  {
    public static ClaimsIdentity GetClaimsIdentity(this ApiKey apiKey, string? authenticationScheme = null)
    {
      ArgumentNullException.ThrowIfNull(apiKey);

      var identity = new ClaimsIdentity(authenticationScheme);

      identity.AddClaim(new(Rfc7519ClaimTypes.Subject, apiKey.Id.ToString()));
      identity.AddClaim((apiKey.UpdatedAt ?? apiKey.CreatedAt).GetClaim(Rfc7519ClaimTypes.UpdatedAt));
      identity.AddClaim(new(Rfc7519ClaimTypes.FullName, apiKey.Name));
      identity.AddClaim(DateTime.UtcNow.GetClaim(Rfc7519ClaimTypes.AuthenticationTime));

      if (apiKey.ExpiresAt.HasValue)
        identity.AddClaim(apiKey.ExpiresAt.Value.GetClaim(Rfc7519ClaimTypes.Expires));

      return identity;
    }
    public static ClaimsIdentity GetClaimsIdentity(this User user, string? authenticationScheme = null)
    {
      ArgumentNullException.ThrowIfNull(user);

      var identity = new ClaimsIdentity(authenticationScheme);

      identity.AddClaim(new(Rfc7519ClaimTypes.Subject, user.Id.ToString()));
      identity.AddClaim(new(Rfc7519ClaimTypes.Username, user.Username));
      identity.AddClaim((user.UpdatedAt ?? user.CreatedAt).GetClaim(Rfc7519ClaimTypes.UpdatedAt));

      if (user.Email != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.Email, user.Email));
        identity.AddClaim(new(Rfc7519ClaimTypes.EmailVerified, user.EmailConfirmedAt.HasValue.ToString().ToLower(), ClaimValueTypes.Boolean));
      }
      if (user.PhoneNumber != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.PhoneNumber, user.PhoneNumber));
        identity.AddClaim(new(Rfc7519ClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmedAt.HasValue.ToString().ToLower(), ClaimValueTypes.Boolean));
      }

      if (user.FullName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.FullName, user.FullName));

        if (user.FirstName != null)
          identity.AddClaim(new(Rfc7519ClaimTypes.FirstName, user.FirstName));
        if (user.MiddleName != null)
          identity.AddClaim(new(Rfc7519ClaimTypes.MiddleName, user.MiddleName));
        if (user.LastName != null)
          identity.AddClaim(new(Rfc7519ClaimTypes.LastName, user.LastName));
      }

      if (user.Culture != null)
        identity.AddClaim(new(Rfc7519ClaimTypes.Locale, user.Culture.Name));
      if (user.Picture != null)
        identity.AddClaim(new(Rfc7519ClaimTypes.PictureUri, user.Picture));

      if (user.SignedInAt.HasValue)
        identity.AddClaim(user.SignedInAt.Value.GetClaim(Rfc7519ClaimTypes.AuthenticationTime));

      return identity;
    }
    private static Claim GetClaim(this DateTime moment, string type)
    {
      return new(type, new DateTimeOffset(moment).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);
    }

    public static DateTime GetDateTime(this Claim claim)
    {
      ArgumentNullException.ThrowIfNull(claim);

      return DateTimeOffset.FromUnixTimeSeconds(long.Parse(claim.Value)).UtcDateTime;
    }
  }
}
