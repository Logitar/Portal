using AutoMapper;
using FluentValidation;
using Portal.Core.Realms;
using Portal.Core.Sessions;
using Portal.Core.Users.Models;
using Portal.Core.Users.Payloads;

namespace Portal.Core.Users
{
  internal class UserService : IUserService
  {
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
        realm = (Guid.TryParse(payload.Realm, out Guid guid)
          ? await _realmQuerier.GetAsync(guid, readOnly: false, cancellationToken)
          : await _realmQuerier.GetAsync(alias: payload.Realm, readOnly: false, cancellationToken)
        ) ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
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

      string passwordHash = _passwordService.Hash(payload.Password);
      payload.Password = string.Empty;

      var user = new User(payload, _userContext.ActorId, passwordHash, realm);

      if (payload.ConfirmEmail)
      {
        user.ConfirmEmail(_userContext.ActorId);
      }
      if (payload.ConfirmPhoneNumber)
      {
        user.ConfirmPhoneNumber(_userContext.ActorId);
      }

      var context = ValidationContext<User>.CreateWithOptions(user, options => options.ThrowOnFailures());
      context.SetAllowedUsernameCharacters(realm?.AllowedUsernameCharacters);
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

      IEnumerable<Session> sessions = await _sessionQuerier.GetActiveAsync(user, readOnly: false, cancellationToken);
      foreach (Session session in sessions)
      {
        session.SignOut(_userContext.ActorId);
      }
      await _sessionRepository.SaveAsync(sessions, cancellationToken);

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

    public async Task<ListModel<UserModel>> GetAsync(bool? isConfirmed, bool? isDisabled, Guid? realmId, string? search,
      UserSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      PagedList<User> users = await _querier.GetPagedAsync(isConfirmed, isDisabled, realmId, search,
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

      user.Update(payload, _userContext.ActorId);
      _validator.ValidateAndThrow(user);

      await _repository.SaveAsync(user, cancellationToken);

      return _mapper.Map<UserModel>(user);
    }
  }
}
