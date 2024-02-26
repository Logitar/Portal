using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Queries;

internal class ReadOneTimePasswordQueryHandler : IRequestHandler<ReadOneTimePasswordQuery, OneTimePassword?>
{
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;

  public ReadOneTimePasswordQueryHandler(IOneTimePasswordQuerier oneTimePasswordQuerier)
  {
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
  }

  public async Task<OneTimePassword?> Handle(ReadOneTimePasswordQuery query, CancellationToken cancellationToken)
  {
    return await _oneTimePasswordQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
