using Bogus;
using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Contracts.Users.Contact;

namespace Logitar.Portal.Client;

internal class UserServiceTests
{
  private readonly Faker _faker = new();

  private readonly TestContext _context;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public UserServiceTests(TestContext context, ITokenService tokenService, IUserService userService)
  {
    _context = context;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<User?> ExecuteAsync(Credentials credentials, CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      string realm = _context.Realm.Id.ToString();

      name = string.Join('.', nameof(UserService), nameof(UserService.CreateAsync));
      CreateUserInput input = new()
      {
        Realm = realm,
        Username = credentials.Username
      };
      User user = await _userService.CreateAsync(input, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.UpdateAsync));
      string current = $"{credentials.Password}!";
      UpdateUserInput update = new()
      {
        Password = current,
        Address = new AddressInput
        {
          Line1 = "1909 Av. des Canadiens-de-Montréal",
          Locality = "Montréal",
          PostalCode = "H3B 5E8",
          Country = "CA",
          Region = "QC"
        },
        Email = new EmailInput
        {
          Address = _faker.Person.Email
        },
        Phone = new PhoneInput
        {
          CountryCode = "+1",
          Number = "(514) 932-2582"
        },
        FirstName = _faker.Person.FirstName,
        LastName = _faker.Person.LastName,
        Birthdate = _faker.Person.DateOfBirth,
        Gender = _faker.Person.Gender.ToString(),
        Locale = _context.Realm.DefaultLocale,
        TimeZone = "America/Montreal",
        Picture = "https://centrebell.ca/img/logo.svg",
        Profile = "https://centrebell.ca/fr/contact",
        Website = "https://www.centrebell.ca/",
        CustomAttributes = new[]
        {
          new CustomAttribute
          {
            Key = "Company",
            Value = "Centre Bell"
          }
        }
      };
      user = await _userService.UpdateAsync(user.Id, update, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.GetAsync));
      user = (await _userService.GetAsync(realm: realm, skip: 0, cancellationToken: cancellationToken)).Items.Single();
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), $"{nameof(UserService.GetAsync)}(id)");
      user = await _userService.GetAsync(user.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result cannot be null.");
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.DeleteAsync));
      Guid deleteId = (await _userService.CreateAsync(new CreateUserInput
      {
        Realm = realm,
        Username = "delete-me"
      }, cancellationToken)).Id;
      _ = await _userService.DeleteAsync(deleteId, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.ChangePasswordAsync));
      user = await _userService.ChangePasswordAsync(user.Id, new ChangePasswordInput
      {
        Current = current,
        Password = $"+{current}"
      }, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.DisableAsync));
      user = await _userService.DisableAsync(user.Id, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.EnableAsync));
      user = await _userService.EnableAsync(user.Id, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.VerifyAddressAsync));
      user = await _userService.VerifyAddressAsync(user.Id, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.VerifyEmailAsync));
      user = await _userService.VerifyEmailAsync(user.Id, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.VerifyPhoneAsync));
      user = await _userService.VerifyPhoneAsync(user.Id, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.SetExternalIdentifierAsync));
      user = await _userService.SetExternalIdentifierAsync(user.Id, key: "EmployeeId", value: Guid.NewGuid().ToString(), cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.RecoverPasswordAsync));
      SentMessages sentMessages = await _userService.RecoverPasswordAsync(new RecoverPasswordInput
      {
        Realm = realm,
        Username = user.Email?.Address ?? string.Empty
      }, cancellationToken);
      _ = sentMessages.Success.Single();
      _context.Succeed(name);

      name = string.Join('.', nameof(UserService), nameof(UserService.ResetPasswordAsync));
      CreatedToken createdToken = await _tokenService.CreateAsync(new CreateTokenInput
      {
        IsConsumable = true,
        Lifetime = 7 * 24 * 60 * 60,
        Purpose = "reset_password",
        Realm = realm,
        Subject = user.Id.ToString()
      }, cancellationToken);
      user = await _userService.ResetPasswordAsync(new ResetPasswordInput
      {
        Realm = realm,
        Token = createdToken.Token,
        Password = credentials.Password
      }, cancellationToken);
      _context.Succeed(name);

      return user;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);

      return null;
    }
  }
}
