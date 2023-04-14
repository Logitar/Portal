﻿using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users.Contact;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Users.Commands;

internal class UpdateUserHandler : IRequestHandler<UpdateUser, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(UpdateUser request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);
    RealmAggregate? realm = await _realmRepository.LoadAsync(user, cancellationToken);

    UpdateUserInput input = request.Input;

    ReadOnlyEmail? email = ReadOnlyEmail.From(input.Email);
    if (realm?.RequireUniqueEmail == true && email != null)
    {
      IEnumerable<UserAggregate> users = (await _userRepository.LoadAsync(realm, email, cancellationToken));
      if (users.Any(u => !u.Equals(user)))
      {
        throw new EmailAddressAlreadyUsedException(email, nameof(input.Email));
      }
    }

    ReadOnlyAddress? address = ReadOnlyAddress.From(input.Address);
    ReadOnlyPhone? phone = ReadOnlyPhone.From(input.Phone);
    Gender? gender = input.Gender?.GetGender();
    CultureInfo? locale = input.Locale?.GetCultureInfo(nameof(input.Locale));
    TimeZoneEntry? timeZone = input.TimeZone?.GetTimeZoneEntry(nameof(input.TimeZone));
    Uri? picture = input.Picture?.GetUri(nameof(input.Picture));
    Uri? profile = input.Profile?.GetUri(nameof(input.Profile));
    Uri? website = input.Website?.GetUri(nameof(input.Website));

    user.Update(_applicationContext.ActorId, input.FirstName, input.MiddleName, input.LastName, input.Nickname,
      input.Birthdate, gender, locale, timeZone, picture, profile, website,
      input.CustomAttributes?.ToDictionary());

    if (input.Password != null)
    {
      if (realm == null)
      {
        user.ChangePassword(_applicationContext.ActorId, _applicationContext.Configuration.PasswordSettings, input.Password);
      }
      else
      {
        user.ChangePassword(_applicationContext.ActorId, realm, input.Password);
      }
    }

    user.SetAddress(_applicationContext.ActorId, address);
    user.SetEmail(_applicationContext.ActorId, email);
    user.SetPhone(_applicationContext.ActorId, phone);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
