using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Client;

internal class ResetPasswordTests
{
  private const string Sut = "PasswordRecovery";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly IMessageService _messageService;
  private readonly IRealmService _realmService;
  private readonly IUserService _userService;

  public ResetPasswordTests(TestContext context, Faker faker, IMessageService messageService, IRealmService realmService, IUserService userService)
  {
    _context = context;
    _faker = faker;
    _messageService = messageService;
    _realmService = realmService;
    _userService = userService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_userService.RecoverPasswordAsync)}";
      UpdateRealmPayload updateRealm = new()
      {
        PasswordRecoveryTemplateId = new Modification<Guid?>(_context.Template.Id)
      };
      _ = await _realmService.UpdateAsync(_context.Realm.Id, updateRealm, cancellationToken);
      RecoverPasswordPayload recoverPassword = new()
      {
        Realm = _context.Realm.UniqueSlug,
        UniqueName = _context.User.Email?.Address ?? string.Empty,
        IgnoreUserLocale = true,
        Locale = "fr-CA"
      };
      RecoverPasswordResult result = await _userService.RecoverPasswordAsync(recoverPassword, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_userService.ResetPasswordAsync)}";
      Message message = await _messageService.ReadAsync(result.MessageId, cancellationToken)
        ?? throw new InvalidOperationException("The message should not be null.");
      ResetPasswordPayload resetPassword = new()
      {
        Realm = _context.Realm.UniqueSlug,
        Token = message.Variables.Single(v => v.Key == "Token").Value,
        Password = "P@s$W0rD"
      };
      _ = await _userService.ResetPasswordAsync(resetPassword, cancellationToken);
      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
