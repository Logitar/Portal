﻿using Logitar.Portal.Contracts.Users.Models;
using System.Security.Claims;

namespace Logitar.Portal.Application.Claims
{
  public static class ClaimExtensions
  {
    public static ClaimsIdentity GetClaimsIdentity(this UserModel user, string? authenticationScheme = null)
    {
      ArgumentNullException.ThrowIfNull(user);

      var identity = new ClaimsIdentity(authenticationScheme);

      identity.AddClaim(new(Rfc7519ClaimTypes.Subject, user.Id.ToString()));
      identity.AddClaim(new(Rfc7519ClaimTypes.Username, user.Username));
      identity.AddClaim((user.UpdatedOn ?? user.CreatedOn).GetClaim(Rfc7519ClaimTypes.UpdatedAt));

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
    {
      return new(type, new DateTimeOffset(moment).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);
    }
  }
}
