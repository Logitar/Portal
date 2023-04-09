using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users.Contact;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal class UpdateUserHandler : IRequestHandler<UpdateUser, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserHandler(ICurrentActor currentActor,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(UpdateUser request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);
    RealmAggregate realm = await _realmRepository.LoadAsync(user, cancellationToken);

    UpdateUserInput input = request.Input;

    // TODO(fpion): RequireUniqueEmail

    Gender? gender = input.Gender?.GetGender();
    CultureInfo? locale = input.Locale?.GetCultureInfo(nameof(input.Locale));
    TimeZoneEntry? timeZone = input.TimeZone?.GetTimeZoneEntry(nameof(input.TimeZone));
    Uri? picture = input.Picture?.GetUri(nameof(input.Picture));
    Uri? profile = input.Profile?.GetUri(nameof(input.Profile));
    Uri? website = input.Website?.GetUri(nameof(input.Website));

    user.Update(_currentActor.Id, input.FirstName, input.MiddleName, input.LastName, input.Nickname,
      input.Birthdate, gender, locale, timeZone, picture, profile, website,
      input.CustomAttributes?.ToDictionary());

    if (input.Password != null)
    {
      user.ChangePassword(_currentActor.Id, realm, input.Password);
    }

    ReadOnlyAddress? address = input.Address == null ? null : new(input.Address);
    user.SetAddress(_currentActor.Id, address);

    ReadOnlyEmail? email = input.Email == null ? null : new(input.Email);
    user.SetEmail(_currentActor.Id, email);

    ReadOnlyPhone? phone = input.Phone == null ? null : new(input.Phone);
    user.SetPhone(_currentActor.Id, phone);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
