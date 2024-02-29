using Bogus;
using Logitar.Identity.Contracts;
using Logitar.Portal.Client;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Users;

internal class UserClientTests : IClientTests
{
  private readonly Faker _faker = new();

  private readonly IUserClient _client;
  private readonly BasicCredentials _credentials;

  public UserClientTests(IUserClient client, IConfiguration configuration)
  {
    _client = client;

    PortalSettings portalSettings = configuration.GetSection("Portal").Get<PortalSettings>() ?? new();
    _credentials = portalSettings.Basic ?? throw new ArgumentException("The configuration 'Portal.Basic' is required.", nameof(configuration));
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      string password = "Test123!";
      if (password == _credentials.Password)
      {
        password = new string(password.Reverse().ToArray());
      }
      CreateUserPayload create = new(_credentials.Username)
      {
        Password = password
      };
      User user = await _client.CreateAsync(create, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.DeleteAsync));
      user = await _client.DeleteAsync(user.Id, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      user = await _client.CreateAsync(create, context.Request);
      context.User = user;
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ResetPasswordAsync));
      ResetUserPasswordPayload resetPassword = new(_credentials.Password);
      user = await _client.ResetPasswordAsync(user.Id, resetPassword, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.AuthenticateAsync));
      AuthenticateUserPayload authenticate = new(_credentials.Username, _credentials.Password);
      user = await _client.AuthenticateAsync(authenticate, context.Request);
      context.Succeed();

      CustomIdentifier identifier = new("HealthInsuranceNumber", _faker.Person.BuildHealthInsuranceNumber());

      context.SetName(_client.GetType(), nameof(_client.SaveIdentifierAsync));
      SaveUserIdentifierPayload saveIdentifier = new(identifier.Value);
      user = await _client.SaveIdentifierAsync(user.Id, identifier.Key, saveIdentifier, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.RemoveIdentifierAsync));
      user = await _client.RemoveIdentifierAsync(user.Id, identifier.Key, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      user = await _client.SaveIdentifierAsync(user.Id, identifier.Key, saveIdentifier, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      User? notFound = await _client.ReadAsync(Guid.NewGuid(), uniqueName: null, identifier: null, context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The user should not be found.");
      }
      user = await _client.ReadAsync(user.Id, user.UniqueName, identifier, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      context.Succeed();

      long version = user.Version;

      context.SetName(_client.GetType(), nameof(_client.UpdateAsync));
      UpdateUserPayload update = new()
      {
        Password = new ChangePasswordPayload(_credentials.Password)
        {
          Current = _credentials.Password
        },
        Email = new Modification<EmailPayload>(new EmailPayload(_faker.Person.Email, isVerified: true)),
        FirstName = new Modification<string>(_faker.Person.FirstName),
        LastName = new Modification<string>(_faker.Person.LastName)
      };
      if (context.Role == null)
      {
        throw new InvalidOperationException("The role should not be null in the context.");
      }
      update.Roles.Add(new RoleModification(context.Role.UniqueName));
      user = await _client.UpdateAsync(user.Id, update, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchUsersPayload search = new()
      {
        HasAuthenticated = true,
        HasPassword = true,
        IsDisabled = false,
        IsConfirmed = true
      };
      search.Search.Terms.Add(new SearchTerm(_faker.Person.FirstName));
      search.Search.Terms.Add(new SearchTerm(_faker.Person.LastName));
      SearchResults<User> results = await _client.SearchAsync(search, context.Request);
      user = results.Items.Single();
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReplaceAsync));
      if (user.Email == null)
      {
        throw new InvalidOperationException("The user email should not be null.");
      }
      ReplaceUserPayload replace = new(user.UniqueName)
      {
        Password = null,
        IsDisabled = false,
        Address = user.Address == null ? null : new AddressPayload(user.Address.Street, user.Address.Locality, user.Address.PostalCode, user.Address.Region, user.Address.Country, user.Address.IsVerified),
        Email = new EmailPayload(user.Email.Address, user.Email.IsVerified),
        Phone = user.Phone == null ? null : new PhonePayload(user.Phone.CountryCode, user.Phone.Number, user.Phone.Extension, user.Phone.IsVerified),
        FirstName = user.FirstName,
        MiddleName = user.MiddleName,
        LastName = user.LastName,
        Nickname = user.Nickname,
        Birthdate = _faker.Person.DateOfBirth,
        Gender = _faker.Person.Gender.ToString(),
        Locale = "fr-CA",
        TimeZone = "America/Toronto",
        Picture = _faker.Person.Avatar,
        Website = $"https://www.{_faker.Person.Website}",
        CustomAttributes = user.CustomAttributes,
        Roles = user.Roles.Select(role => role.UniqueName).ToList()
      };
      user = await _client.ReplaceAsync(user.Id, replace, version, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SignOutAsync));
      user = await _client.SignOutAsync(user.Id, context.Request)
        ?? throw new InvalidOperationException("The user should not be null.");
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
