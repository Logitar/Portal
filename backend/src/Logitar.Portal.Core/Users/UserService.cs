using AutoMapper;
using FluentValidation;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;

namespace Logitar.Portal.Core.Users
{
  internal class UserService : IUserService
  {
    private const string AllowedUsernameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly IUserQuerier _querier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<User> _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<User> _validator;

    public UserService(
      IMapper mapper,
      IPasswordService passwordService,
      IUserQuerier querier,
      IRealmQuerier realmQuerier,
      IRepository<User> repository,
      IUserContext userContext,
      IValidator<User> validator
    )
    {
      _mapper = mapper;
      _passwordService = passwordService;
      _querier = querier;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _userContext = userContext;
      _validator = validator;
    }

    public async Task<UserModel> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _realmQuerier.GetAsync(payload.Realm, readOnly: false, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      _passwordService.ValidateAndThrow(payload.Password, realm);

      if (await _querier.GetAsync(payload.Username, realm, readOnly: true, cancellationToken) != null)
      {
        throw new UsernameAlreadyUsedException(payload.Username, nameof(payload.Username));
      }
      else if (realm?.RequireUniqueEmail == true
        && payload.Email != null
        && (await _querier.GetByEmailAsync(payload.Email, realm, readOnly: true, cancellationToken)).Any())
      {
        throw new EmailAlreadyUsedException(payload.Email, nameof(payload.Email));
      }

      var securePayload = _mapper.Map<CreateUserSecurePayload>(payload);
      securePayload.PasswordHash = _passwordService.Hash(payload.Password);
      var user = new User(securePayload, _userContext.ActorId, realm);

      if (payload.ConfirmEmail)
      {
        user.ConfirmEmail(_userContext.ActorId);
      }
      if (payload.ConfirmPhoneNumber)
      {
        user.ConfirmPhoneNumber(_userContext.ActorId);
      }

      var context = ValidationContext<User>.CreateWithOptions(user, options => options.ThrowOnFailures());
      context.SetAllowedUsernameCharacters(realm == null ? AllowedUsernameCharacters : realm.AllowedUsernameCharacters);
      _validator.Validate(context);

      await _repository.SaveAsync(user, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }

    public async Task<UserModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
      User user = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<User>(id);

      user.Delete(_userContext.ActorId);

      await _repository.SaveAsync(user, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }

    public async Task<UserModel> DisableAsync(Guid id, CancellationToken cancellationToken = default)
    {
      User user = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<User>(id);

      if (user.IsDisabled)
      {
        throw new UserAlreadyDisabledException(user);
      }

      user.Disable(_userContext.ActorId);
      _validator.ValidateAndThrow(user);

      await _repository.SaveAsync(user, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }

    public async Task<UserModel> EnableAsync(Guid id, CancellationToken cancellationToken = default)
    {
      User user = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<User>(id);

      if (!user.IsDisabled)
      {
        throw new UserNotDisabledException(user);
      }

      user.Enable(_userContext.ActorId);
      _validator.ValidateAndThrow(user);

      await _repository.SaveAsync(user, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }

    public async Task<UserModel?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
      User? user = await _querier.GetAsync(id, readOnly: true, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }

    public async Task<ListModel<UserModel>> GetAsync(bool? isConfirmed, bool? isDisabled, string? realm, string? search,
      UserSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<User> users = await _querier.GetPagedAsync(isConfirmed, isDisabled, realm, search,
        sort, desc,
        index, count,
        readOnly: true, cancellationToken);

      return ListModel<UserModel>.From(users, _mapper);
    }

    public async Task<UserModel> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      User user = await _querier.GetAsync(id, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<User>(id);

      var securePayload = _mapper.Map<UpdateUserSecurePayload>(payload);

      if (payload.Password != null)
      {
        _passwordService.ValidateAndThrow(payload.Password, user.Realm);
        securePayload.PasswordHash = _passwordService.Hash(payload.Password);
      }

      user.Update(securePayload, _userContext.ActorId);
      _validator.ValidateAndThrow(user);

      await _repository.SaveAsync(user, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }
  }
}
