﻿using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands
{
  internal class EnableUserCommandHandler : IRequestHandler<EnableUserCommand, UserModel>
  {
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IUserValidator _userValidator;

    public EnableUserCommandHandler(IRepository repository,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IUserValidator userValidator)
    {
      _repository = repository;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _userValidator = userValidator;
    }

    public async Task<UserModel> Handle(EnableUserCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<User>(request.Id);

      user.Enable(_userContext.ActorId);
      await _userValidator.ValidateAndThrowAsync(user, cancellationToken);

      await _repository.SaveAsync(user, cancellationToken);

      return await _userQuerier.GetAsync(user.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
    }
  }
}