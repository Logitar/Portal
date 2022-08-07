using AutoMapper;
using FluentValidation;
using Logitar.Portal.Core.Actors;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;

namespace Logitar.Portal.Core.Users
{
  internal class UserService : IUserService
  {
    private const string AllowedUsernameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    private readonly IActorService _actorService;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly IUserQuerier _querier;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository<User> _repository;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly IRepository<Session> _sessionRepository;
    private readonly IUserContext _userContext;
    private readonly IValidator<User> _validator;

    public UserService(
      IActorService actorService,
      IMapper mapper,
      IPasswordService passwordService,
      IUserQuerier querier,
      IRealmQuerier realmQuerier,
      IRepository<User> repository,
      ISessionQuerier sessionQuerier,
      IRepository<Session> sessionRepository,
      IUserContext userContext,
      IValidator<User> validator
    )
    {
      _actorService = actorService;
      _mapper = mapper;
      _passwordService = passwordService;
      _querier = querier;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _sessionQuerier = sessionQuerier;
      _sessionRepository = sessionRepository;
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

      if (payload.Password != null)
      {
        _passwordService.ValidateAndThrow(payload.Password, realm);
        securePayload.PasswordHash = _passwordService.Hash(payload.Password);
      }

      var user = new User(securePayload, _userContext.Actor.Id, realm);
      if (payload.ConfirmEmail)
      {
        user.ConfirmEmail(_userContext.Actor.Id);
      }
      if (payload.ConfirmPhoneNumber)
      {
        user.ConfirmPhoneNumber(_userContext.Actor.Id);
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

      if (user.Id == _userContext.Actor.Id)
      {
        throw new UserCannotDeleteHerselfException(user);
      }

      PagedList<Session> sessions = await _sessionQuerier.GetPagedAsync(userId: user.Id, readOnly: false, cancellationToken: cancellationToken);
      foreach (Session session in sessions)
      {
        session.Delete(_userContext.Actor.Id);
      }
      await _sessionRepository.SaveAsync(sessions, cancellationToken);

      user.Delete(_userContext.Actor.Id);

      await _repository.SaveAsync(user, cancellationToken);
      await _actorService.SaveAsync(user, cancellationToken);

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
      else if (user.Id == _userContext.Actor.Id)
      {
        throw new UserCannotDisableHerselfException(user);
      }

      user.Disable(_userContext.Actor.Id);
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

      user.Enable(_userContext.Actor.Id);
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

      if (user.Realm?.RequireUniqueEmail == true && payload.Email != null)
      {
        IEnumerable<User> users = await _querier.GetByEmailAsync(payload.Email, user.Realm, readOnly: true, cancellationToken);
        if (users.Any(x => !x.Equals(user)))
        {
          throw new EmailAlreadyUsedException(payload.Email, nameof(payload.Email));
        }
      }

      var securePayload = _mapper.Map<UpdateUserSecurePayload>(payload);

      if (payload.Password != null)
      {
        _passwordService.ValidateAndThrow(payload.Password, user.Realm);
        securePayload.PasswordHash = _passwordService.Hash(payload.Password);
      }

      user.Update(securePayload, _userContext.Actor.Id);
      _validator.ValidateAndThrow(user);

      await _repository.SaveAsync(user, cancellationToken);
      await _actorService.SaveAsync(user, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }
  }
}
