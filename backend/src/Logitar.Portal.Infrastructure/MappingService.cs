using AutoMapper;

namespace Logitar.Portal.Infrastructure
{
  internal class MappingService : IMappingService
  {
    private readonly IMapper _mapper;

    public MappingService(IMapper mapper)
    {
      _mapper = mapper;
    }

    /// <summary>
    /// TODO(fpion): implement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T?> MapAsync<T>(object? source, CancellationToken cancellationToken)
    {
      return _mapper.Map<T>(source);
    }

    /// <summary>
    /// TODO(fpion): implement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sources"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<T>> MapAsync<T>(IEnumerable<object?> sources, CancellationToken cancellationToken = default)
    {
      return _mapper.Map<IEnumerable<T>>(sources);
    }
  }
}
