using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Core.Profiles;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Configurations.Queries;

internal class ProjectToConfigurationOutputHandler : IRequestHandler<ProjectToConfigurationOutput, Configuration>
{
  private static readonly Actor _system = new();

  private readonly IMapper _mapper;
  private readonly IUserRepository _userRepository;

  public ProjectToConfigurationOutputHandler(IMapper mapper, IUserRepository userRepository)
  {
    _mapper = mapper;
    _userRepository = userRepository;
  }

  public async Task<Configuration> Handle(ProjectToConfigurationOutput request, CancellationToken cancellationToken)
  {
    ConfigurationAggregate configuration = request.Configuration;

    IReadOnlyDictionary<AggregateId, Actor> actors = await ResolveActorsAsync(new[]
    {
      configuration.CreatedById,
      configuration.UpdatedById
    }, cancellationToken);

    return _mapper.Map<Configuration>(configuration, options => options.Items[MappingExtensions.ActorsKey] = actors);
  }

  private async Task<IReadOnlyDictionary<AggregateId, Actor>> ResolveActorsAsync(IEnumerable<AggregateId> ids, CancellationToken cancellationToken)
  {
    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(ids, includeDeleted: true, cancellationToken);

    Dictionary<AggregateId, Actor> actors = new(capacity: 1 + users.Count())
    {
      [new AggregateId(Guid.Empty)] = _system
    };

    foreach (UserAggregate user in users)
    {
      actors[user.Id] = new Actor
      {
        Id = user.Id.ToGuid(),
        Type = ActorType.User,
        IsDeleted = user.IsDeleted,
        DisplayName = user.FullName ?? user.Username,
        EmailAddress = user.Email?.Address,
        Picture = user.Picture?.ToString()
      };
    }

    return actors;
  }
}
