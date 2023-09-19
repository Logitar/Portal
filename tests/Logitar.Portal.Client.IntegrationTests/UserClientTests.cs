using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Client;

internal class UserClientTests
{
  private const string CurrentPassword = "Test123!";
  private const string NewPassword = "P@s$W0rD";
  private const string Sut = "UserClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly IUserService _userService;

  public UserClientTests(TestContext context, Faker faker, IUserService userService)
  {
    _context = context;
    _faker = faker;
    _userService = userService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_userService.CreateAsync)}";
      CreateUserPayload create = new()
      {
        Realm = _context.Realm.UniqueSlug,
        UniqueName = _faker.Person.UserName
      };
      User user = await _userService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.ReplaceAsync)}";
      ReplaceUserPayload replace = new()
      {
        UniqueName = user.UniqueName,
        Password = CurrentPassword,
        IsDisabled = true,
        Address = new AddressPayload
        {
          Street = "150 Saint-Catherine St W",
          Locality = "Montreal",
          Region = "QC",
          PostalCode = "H2X 3Y2",
          Country = "CA"
        },
        Email = new EmailPayload
        {
          Address = _faker.Person.Email
        },
        Phone = new PhonePayload
        {
          CountryCode = "CA",
          Number = "+15148454636",
          Extension = "123456"
        },
        FirstName = _faker.Person.FirstName,
        MiddleName = _faker.Name.FirstName(_faker.Person.Gender),
        LastName = _faker.Person.LastName,
        Nickname = string.Concat(_faker.Person.FirstName.First(), _faker.Person.LastName).ToLower(),
        Birthdate = _faker.Person.DateOfBirth,
        Gender = _faker.Person.Gender.ToString(),
        Locale = _faker.Locale,
        TimeZone = "America/Montreal",
        Picture = _faker.Person.Avatar,
        Profile = $"{_context.Realm.Url?.ToString().TrimEnd('/')}/employees/{user.Id}",
        Website = $"https://www.{_faker.Person.Website}/",
        CustomAttributes = new CustomAttribute[]
        {
          new("DepartmentCode", "009"),
          new("EmployeeNo", "733-511177-7"),
          new("HourlyRate", "25.00")
        },
        Roles = new string[] { $"  {_context.Role.UniqueName}  " }
      };
      user = await _userService.ReplaceAsync(user.Id, replace, user.Version, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.UpdateAsync)}";
      UpdateUserPayload update = new()
      {
        Password = new ChangePasswordPayload
        {
          CurrentPassword = CurrentPassword,
          NewPassword = NewPassword
        },
        IsDisabled = false,
        Email = new Modification<EmailPayload>(new()
        {
          Address = _faker.Person.Email,
          IsVerified = true
        }),
        CustomAttributes = new CustomAttributeModification[]
        {
          new("DepartmentName", "Mortgages"),
          new("HourlyRate", "37.50"),
          new("DepartmentCode", value: null)
        }
      };
      user = await _userService.UpdateAsync(user.Id, update, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.DeleteAsync)}";
      User delete = await _userService.CreateAsync(new CreateUserPayload
      {
        Realm = _context.Realm.Id.ToString(),
        UniqueName = $"{user.UniqueName}2"
      }, cancellationToken);
      delete = await _userService.DeleteAsync(delete.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.SearchAsync)}";
      SearchUsersPayload search = new()
      {
        Realm = $"  {_context.Realm.UniqueSlug}  ",
        IdIn = new Guid[] { user.Id }
      };
      SearchResults<User> results = await _userService.SearchAsync(search, cancellationToken);
      user = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.SaveIdentifierAsync)}";
      SaveIdentifierPayload saveIdentifier = new()
      {
        Key = "EmployeeNo",
        Value = "733-511177-7"
      };
      user = await _userService.SaveIdentifierAsync(user.Id, saveIdentifier, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.ReadAsync)}:null";
      User? result = await _userService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The user should be null.");
      }
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.ReadAsync)}:Id";
      user = await _userService.ReadAsync(user.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.ReadAsync)}:UniqueName";
      user = await _userService.ReadAsync(realm: _context.Realm.UniqueSlug, uniqueName: user.UniqueName, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.ReadAsync)}:Identifier";
      user = await _userService.ReadAsync(realm: _context.Realm.UniqueSlug, identifierKey: saveIdentifier.Key,
        identifierValue: $"  {saveIdentifier.Value.ToUpper()}  ", cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.RemoveIdentifierAsync)}";
      user = await _userService.RemoveIdentifierAsync(user.Id, saveIdentifier.Key, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.AuthenticateAsync)}";
      AuthenticateUserPayload authenticate = new()
      {
        Realm = _context.Realm.Id.ToString(),
        UniqueName = $"  {user.UniqueName.ToUpper()}  ",
        Password = NewPassword
      };
      user = await _userService.AuthenticateAsync(authenticate, cancellationToken);
      _context.Succeed(name);

      _context.User = user;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }

  public async Task<bool> SignOutAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;
    try
    {
      name = $"{Sut}.{nameof(_userService.SignOutAsync)}:Id";
      User user = await _userService.SignOutAsync(_context.User.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
