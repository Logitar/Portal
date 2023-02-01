using Logitar.Portal.Contracts.Users.Models;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries
{
  internal record GetUserQuery(string Id) : IRequest<UserModel>;
}
