using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Senders.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Emails.Senders;

namespace Logitar.Portal.Application.Emails.Senders
{
  internal class SenderProfile : Profile
  {
    public SenderProfile()
    {
      CreateMap<Sender, SenderModel>()
        .IncludeBase<Aggregate, AggregateModel>()
        .ForMember(x => x.Settings, x => x.MapFrom(y => y.Settings.Select(pair => new SettingModel
        {
          Key = pair.Key,
          Value = pair.Value
        })));
      CreateMap<SenderModel, SenderSummary>()
        .IncludeBase<AggregateModel, AggregateSummary>();
    }
  }
}
