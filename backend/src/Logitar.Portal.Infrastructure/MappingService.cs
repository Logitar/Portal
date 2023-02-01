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
  }
}
