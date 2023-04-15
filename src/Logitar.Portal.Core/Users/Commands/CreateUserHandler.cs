using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users.Contact;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Users.Commands;

internal class CreateUserHandler : IRequestHandler<CreateUser, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public CreateUserHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(CreateUser request, CancellationToken cancellationToken)
  {
    CreateUserInput input = request.Input;

    RealmAggregate? realm = await LoadRealmAsync(input, cancellationToken);

    string username = input.Username.Trim();
    if (await _userRepository.LoadAsync(realm, username, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(username, nameof(input.Username));
    }

    ReadOnlyEmail? email = ReadOnlyEmail.From(input.Email);
    if (realm?.RequireUniqueEmail == true && email != null)
    {
      if ((await _userRepository.LoadAsync(realm, email, cancellationToken)).Any())
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

    UserAggregate user;
    if (realm == null)
    {
      ConfigurationAggregate configuration = _applicationContext.Configuration;
      user = new(_applicationContext.ActorId, configuration.UsernameSettings, username, input.FirstName, input.MiddleName,
        input.LastName, input.Nickname, input.Birthdate, gender, locale, timeZone,
        picture, profile, website, input.CustomAttributes?.ToDictionary());

      if (input.Password != null)
      {
        user.ChangePassword(_applicationContext.ActorId, configuration.PasswordSettings, input.Password);
      }
    }
    else
    {
      user = new(_applicationContext.ActorId, realm, username, input.FirstName, input.MiddleName,
        input.LastName, input.Nickname, input.Birthdate, gender, locale, timeZone,
        picture, profile, website, input.CustomAttributes?.ToDictionary());

      if (input.Password != null)
      {
        user.ChangePassword(_applicationContext.ActorId, realm, input.Password);
      }
    }

    if (address != null)
    {
      user.SetAddress(_applicationContext.ActorId, address);
    }
    if (email != null)
    {
      user.SetEmail(_applicationContext.ActorId, email);
    }
    if (phone != null)
    {
      user.SetPhone(_applicationContext.ActorId, phone);
    }

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }

  private async Task<RealmAggregate?> LoadRealmAsync(CreateUserInput input, CancellationToken cancellationToken)
  {
    if (input.Realm == null)
    {
      return null;
    }

    return await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));
  }
}
