using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  internal class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<ExternalProvider, ExternalProviderModel>();
      CreateMap<User, UserModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<UserModel, UserSummary>()
        .IncludeBase<AggregateModel, AggregateSummary>();

      CreateMap<CreateUserPayload, CreateUserSecurePayload>();
      CreateMap<UpdateUserPayload, UpdateUserSecurePayload>();
    }
  }
}
