using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users.Contact;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal class CreateUserHandler : IRequestHandler<CreateUser, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public CreateUserHandler(ICurrentActor currentActor,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(CreateUser request, CancellationToken cancellationToken)
  {
    CreateUserInput input = request.Input;

    RealmAggregate realm = await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));

    string username = input.Username.Trim();
    if (await _userRepository.LoadAsync(realm, username, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(username, nameof(input.Username));
    }

    ReadOnlyEmail? email = ReadOnlyEmail.From(input.Email);
    if (realm.RequireUniqueEmail && email != null)
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

    UserAggregate user = new(_currentActor.Id, realm, username, input.FirstName, input.MiddleName,
      input.LastName, input.Nickname, input.Birthdate, gender, locale, timeZone,
      picture, profile, website, input.CustomAttributes?.ToDictionary());

    if (input.Password != null)
    {
      user.ChangePassword(_currentActor.Id, realm, input.Password);
    }

    if (address != null)
    {
      user.SetAddress(_currentActor.Id, address);
    }
    if (email != null)
    {
      user.SetEmail(_currentActor.Id, email);
    }
    if (phone != null)
    {
      user.SetPhone(_currentActor.Id, phone);
    }

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
