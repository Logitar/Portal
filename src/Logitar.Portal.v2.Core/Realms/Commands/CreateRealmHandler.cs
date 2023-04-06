﻿using FluentValidation;
using FluentValidation.Results;
using Logitar.Portal.v2.Contracts.Realms;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Realms.Commands;

internal class CreateRealmHandler : IRequestHandler<CreateRealm, Realm>
{
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public CreateRealmHandler(IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm> Handle(CreateRealm request, CancellationToken cancellationToken)
  {
    CreateRealmInput input = request.Input;

    string uniqueName = input.UniqueName.Trim();
    if (await _realmRepository.LoadByUniqueNameAsync(uniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(uniqueName, nameof(input.UniqueName));
    }

    RealmAggregate realm = new(uniqueName, input.DisplayName, input.Description,
      GetCultureInfo(input.DefaultLocale), input.Secret, GetUri(input.Url),
      input.RequireConfirmedAccount, input.RequireUniqueEmail,
      GetUsernameSettings(input.UsernameSettings), GetPasswordSettings(input.PasswordSettings),
      GetClaimMappings(input.ClaimMappings), GetCustomAttributes(input.CustomAttributes));

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.GetAsync(realm, cancellationToken);
  }

  private static Dictionary<string, ReadOnlyClaimMapping>? GetClaimMappings(IEnumerable<ClaimMapping>? claimMappings)
    => claimMappings?.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Type))
      .GroupBy(x => x.Key.Trim())
      .ToDictionary(x => x.Key, x => new ReadOnlyClaimMapping(x.Last().Type, x.Last().ValueType));

  private static CultureInfo? GetCultureInfo(string? locale) => locale == null ? null : CultureInfo.GetCultureInfo(locale);

  private static Dictionary<string, string>? GetCustomAttributes(IEnumerable<CustomAttribute>? customAttributes)
    => customAttributes?.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
      .GroupBy(x => x.Value.Trim())
      .ToDictionary(x => x.Key, x => x.Last().Value);

  private static Uri? GetUri(string? url)
  {
    try
    {
      return url == null ? null : new Uri(url);
    }
    catch (Exception)
    {
      ValidationFailure error = new(nameof(CreateRealmInput.Url), $"'{nameof(CreateRealmInput.Url)}' must be a valid URL.", url);
      throw new ValidationException(new[] { error });
    }
  }

  private static ReadOnlyUsernameSettings? GetUsernameSettings(UsernameSettings? usernameSettings)
    => usernameSettings == null ? null : new(usernameSettings.AllowedCharacters);

  private static ReadOnlyPasswordSettings? GetPasswordSettings(PasswordSettings? passwordSettings)
    => passwordSettings == null ? null : new(passwordSettings.RequiredLength, passwordSettings.RequiredUniqueChars,
      passwordSettings.RequireNonAlphanumeric, passwordSettings.RequireLowercase, passwordSettings.RequireUppercase,
      passwordSettings.RequireDigit);
}
