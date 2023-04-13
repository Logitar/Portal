using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Realms;
using System.Security.Claims;

namespace Logitar.Portal.Core.Claims;

public static class ClaimExtensions
{
  public static string GetAudience(this RealmAggregate realm)
  {
    return (realm.Url?.ToString() ?? realm.UniqueName).ToLower();
  }
  public static string GetIssuer(this RealmAggregate realm, Uri? baseUrl)
  {
    if (realm.UniqueName != Constants.PortalRealm.UniqueName && baseUrl != null)
    {
      return $"{baseUrl.ToString().TrimEnd('/')}/realms/{{UNIQUE_NAME}}".Format(realm);
    }

    return (realm.Url?.ToString() ?? realm.UniqueName).ToLower();
  }
  public static string Format(this string value, RealmAggregate? realm)
  {
    return (realm == null
      ? value
      : value.Replace("{UNIQUE_NAME}", realm.UniqueName).Replace("{URL}", realm.Url?.ToString())
    ).ToLower();
  }

  public static ClaimsIdentity GetClaimsIdentity(this User user, string? authenticationType = null)
  {
    ClaimsIdentity identity = new(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimTypes.Subject, user.Id.ToString()));
    identity.AddClaim(new(Rfc7519ClaimTypes.Username, user.Username));
    identity.AddClaim(user.UpdatedOn.GetClaim(Rfc7519ClaimTypes.UpdatedOn));

    if (user.Address != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Address, Rfc7519Address.From(user.Address).Serialize()));
      identity.AddClaim(new(Rfc7519ClaimTypes.IsAddressVerified, user.Address.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }
    if (user.Email != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.EmailAddress, user.Email.Address));
      identity.AddClaim(new(Rfc7519ClaimTypes.IsEmailVerified, user.Email.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }
    if (user.Phone != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.PhoneNumber, user.Phone.E164Formatted));
      identity.AddClaim(new(Rfc7519ClaimTypes.IsPhoneVerified, user.Phone.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
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
    if (user.Nickname != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Nickname, user.Nickname));
    }

    if (user.Birthdate.HasValue)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Birthdate, user.Birthdate.Value.ToString("yyyy-MM-dd")));
    }
    if (user.Gender != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Gender, user.Gender.ToLower()));
    }

    if (user.Locale != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Locale, user.Locale));
    }
    if (user.TimeZone != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.TimeZone, user.TimeZone));
    }

    if (user.Picture != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Picture, user.Picture));
    }
    if (user.Profile != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Profile, user.Profile));
    }
    if (user.Website != null)
    {
      identity.AddClaim(new(Rfc7519ClaimTypes.Website, user.Website));
    }

    if (user.SignedInOn.HasValue)
    {
      identity.AddClaim(user.SignedInOn.Value.GetClaim(Rfc7519ClaimTypes.AuthenticationTime));
    }

    return identity;
  }

  private static Claim GetClaim(this DateTime moment, string type)
  {
    return new(type, new DateTimeOffset(moment).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);
  }
  public static DateTime GetDateTime(this Claim claim)
  {
    return DateTimeOffset.FromUnixTimeSeconds(long.Parse(claim.Value)).UtcDateTime;
  }
}
