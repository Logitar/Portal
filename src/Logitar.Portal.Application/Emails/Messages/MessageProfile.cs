using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Messages.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Emails.Messages;

namespace Logitar.Portal.Application.Emails.Messages
{
  internal class MessageProfile : Profile
  {
    public MessageProfile()
    {
      CreateMap<Message, MessageModel>()
        .IncludeBase<Aggregate, AggregateModel>()
        .ForMember(x => x.Result, x => x.MapFrom(y => y.Result == null ? null : y.Result.Select(pair => new ResultDataModel
        {
          Key = pair.Key,
          Value = pair.Value
        })))
        .ForMember(x => x.Variables, x => x.MapFrom(y => y.Variables == null ? null : y.Variables.Select(pair => new VariableModel
        {
          Key = pair.Key,
          Value = pair.Value
        })));
      CreateMap<MessageModel, MessageSummary>()
        .IncludeBase<AggregateModel, AggregateSummary>()
        .ForMember(x => x.Recipients, x => x.MapFrom(y => y.Recipients.Count()));
      CreateMap<Recipient, RecipientModel>();
    }
  }
}
