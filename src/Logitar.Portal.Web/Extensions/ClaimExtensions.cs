﻿using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;

namespace Logitar.Portal.Web.Extensions;

internal static class ClaimExtensions
{
  public static ClaimsIdentity CreateClaimsIdentity(this User user, string? authenticationType = null)
  {
    ClaimsIdentity identity = new(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimNames.Subject, user.Id.ToString()));
    identity.AddClaim(new(Rfc7519ClaimNames.Username, user.UniqueName));
    identity.AddClaim(user.UpdatedOn.CreateClaim(Rfc7519ClaimNames.UpdatedAt));

    if (user.Address != null)
    {
      identity.AddClaim(user.Address.CreateClaim(Rfc7519ClaimNames.Address));
      identity.AddClaim(new(OtherClaimNames.IsAddressVerified, user.Address.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }
    if (user.Email != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.EmailAddress, user.Email.Address));
      identity.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, user.Email.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }
    if (user.Phone != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.PhoneNumber, user.Phone.E164Formatted));
      identity.AddClaim(new(Rfc7519ClaimNames.IsPhoneVerified, user.Phone.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }

    if (user.FullName != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.FullName, user.FullName));

      if (user.FirstName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.FirstName, user.FirstName));
      }

      if (user.MiddleName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.MiddleName, user.MiddleName));
      }

      if (user.LastName != null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.LastName, user.LastName));
      }
    }
    if (user.Nickname != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Nickname, user.Nickname));
    }

    if (user.Birthdate.HasValue)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Birthdate, user.Birthdate.Value.ToString("yyyy-MM-dd")));
    }
    if (user.Gender != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Gender, user.Gender.ToLower()));
    }

    if (user.Locale != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Locale, user.Locale));
    }
    if (user.TimeZone != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.TimeZone, user.TimeZone));
    }

    if (user.Picture != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Picture, user.Picture));
    }
    if (user.Profile != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Profile, user.Profile));
    }
    if (user.Website != null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Website, user.Website));
    }

    if (user.AuthenticatedOn.HasValue)
    {
      identity.AddClaim(user.AuthenticatedOn.Value.CreateClaim(Rfc7519ClaimNames.AuthenticationTime));
    }

    return identity;
  }

  private static Claim CreateClaim(this Address address, string name)
  {
    Rfc7519PostalAddress postalAddress = new()
    {
      Formatted = address.Formatted,
      StreetAddress = address.Street,
      Locality = address.Locality,
      Region = address.Region,
      PostalCode = address.PostalCode,
      Country = address.Country
    };

    return new Claim(name, postalAddress.Serialize());
  }

  private static Claim CreateClaim(this DateTime value, string name)
  {
    return new(name, new DateTimeOffset(value).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);
  }
}