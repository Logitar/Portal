using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, UserModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IUserValidator _userValidator;

    public ChangePasswordCommandHandler(IPasswordService passwordService,
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

    public async Task<UserModel> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(new AggregateId(_userContext.Id), cancellationToken)
        ?? throw new InvalidOperationException($"The user '{_userContext.Id}' could not be found.");

      await _passwordService.ValidateAndThrowAsync(request.Payload.Password, user.RealmId?.Value, cancellationToken);

      if (!_passwordService.IsMatch(user, request.Payload.Current))
      {
        throw new InvalidCredentialsException();
      }

      string passwordHash = _passwordService.Hash(request.Payload.Password);
      user.ChangePassword(passwordHash);
      await _userValidator.ValidateAndThrowAsync(user, cancellationToken);

      await _repository.SaveAsync(user, cancellationToken);

      return await _userQuerier.GetAsync(_userContext.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={_userContext.Id}' could not be found.");
    }
  }
}
