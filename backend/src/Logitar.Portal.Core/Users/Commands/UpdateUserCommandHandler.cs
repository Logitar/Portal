using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Users.Commands
{
  internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IUserValidator _userValidator;

    public UpdateUserCommandHandler(IPasswordService passwordService,
      IRepository repository,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IUserValidator userValidator)
    {
      _passwordService = passwordService;
      _repository = repository;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _userValidator = userValidator;
    }

    public async Task<UserModel> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(new AggregateId(request.Id), cancellationToken)
        ?? throw EntityNotFoundException.Typed<User>(request.Id);

      UpdateUserPayload payload = request.Payload;

      Realm? realm = null;
      if (payload.Email != null && user.RealmId.HasValue)
      {
        realm = await _repository.LoadAsync<Realm>(user.RealmId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm 'Id={user.RealmId}' could not be found.");

        if (realm.RequireUniqueEmail)
        {
          IEnumerable<UserModel> users = await _userQuerier.GetByEmailAsync(payload.Email, realm.Id.ToString(), cancellationToken);
          if (users.Any(x => x.Id != user.Id.Value))
          {
            throw new EmailAlreadyUsedException(payload.Email, nameof(payload.Email));
          }
        }
      }

      string? passwordHash = null;
      if (payload.Password != null)
      {
        if (realm == null)
        {
          await _passwordService.ValidateAndThrowAsync(payload.Password, cancellationToken: cancellationToken);
        }
        else
        {
          _passwordService.ValidateAndThrow(payload.Password, realm);
        }
        passwordHash = _passwordService.Hash(payload.Password);
      }

      CultureInfo? locale = payload.Locale == null ? null : CultureInfo.GetCultureInfo(payload.Locale);
      user.Update(passwordHash, payload.Email, payload.PhoneNumber,
        payload.FirstName, payload.MiddleName, payload.LastName,
        locale, payload.Picture, _userContext.ActorId);
      if (realm == null)
      {
        await _userValidator.ValidateAndThrowAsync(user, cancellationToken);
      }
      else
      {
        _userValidator.ValidateAndThrow(user, realm);
      }

      await _repository.SaveAsync(user, cancellationToken);

      return await _userQuerier.GetAsync(user.Id.ToString(), cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
    }
  }
}
