using AutoMapper;

namespace Logitar.Portal.Infrastructure
{
  /// <summary>
  /// TODO(fpion): implement Actors
  /// </summary>
  internal class MappingService : IMappingService
  {
    private readonly IMapper _mapper;

    public MappingService(IMapper mapper)
    {
      _mapper = mapper;
    }

    public async Task<T?> MapAsync<T>(object? source, CancellationToken cancellationToken)
    {
      return _mapper.Map<T>(source);
    }

    public async Task<IEnumerable<T>> MapAsync<T>(IEnumerable<object?> sources, CancellationToken cancellationToken = default)
    {
      return _mapper.Map<IEnumerable<T>>(sources);
    }
  }
}
