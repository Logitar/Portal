using AutoMapper;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Mappings;

internal class RealmProfile : Profile
{
  public RealmProfile()
  {
    CreateMap<RealmEntity, Realm>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.ClaimMappings, x => x.MapFrom(GetClaimMappings))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(y => MappingHelper.GetCustomAttributes(y.CustomAttributes)));
  }

  private static IEnumerable<ClaimMapping> GetClaimMappings(RealmEntity realm, Realm _)
    => realm.ClaimMappings.Select(claimMapping => new ClaimMapping(claimMapping.Key, claimMapping.Value.Name, claimMapping.Value.Type));
}
