using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Messages;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using System.Text.Json;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Profiles;

internal class MessageProfile : Profile
{
  public MessageProfile()
  {
    CreateMap<MessageEntity, Message>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.GetId))
      .ForMember(x => x.Recipients, x => x.MapFrom(GetRecipients))
      .ForMember(x => x.SenderAddress, x => x.MapFrom(y => y.SenderEmailAddress))
      .ForMember(x => x.TemplateKey, x => x.MapFrom(y => y.TemplateUniqueName))
      .ForMember(x => x.Variables, x => x.MapFrom(GetVariables))
      .ForMember(x => x.Errors, x => x.MapFrom(GetErrors))
      .ForMember(x => x.Result, x => x.MapFrom(GetResult));
    CreateMap<Core.Messages.Recipient, Contracts.Messages.Recipient>();
  }

  private static IEnumerable<Contracts.Messages.Recipient> GetRecipients(MessageEntity message, Message _, object? __, ResolutionContext context)
  {
    IEnumerable<Core.Messages.Recipient> recipients = JsonSerializer.Deserialize<IEnumerable<Core.Messages.Recipient>>(message.Recipients)
      ?? throw new InvalidOperationException($"The recipients deserialization failed: '{message.Recipients}'.");

    return context.Mapper.Map<IEnumerable<Contracts.Messages.Recipient>>(recipients);
  }

  private static IEnumerable<Variable> GetVariables(MessageEntity message, Message _)
  {
    if (message.Variables == null)
    {
      return Enumerable.Empty<Variable>();
    }

    Dictionary<string, string> variables = JsonSerializer.Deserialize<Dictionary<string, string>>(message.Variables)
      ?? throw new InvalidOperationException($"The variables deserialization failed: '{message.Variables}'.");

    return variables.Select(pair => new Variable
    {
      Key = pair.Key,
      Value = pair.Value
    });
  }

  private static IEnumerable<Error> GetErrors(MessageEntity message, Message _)
  {
    if (message.Errors == null)
    {
      return Enumerable.Empty<Error>();
    }

    return JsonSerializer.Deserialize<IEnumerable<Error>>(message.Errors)
      ?? throw new InvalidOperationException($"The variables deserialization failed: '{message.Variables}'.");
  }

  private static IEnumerable<ResultData> GetResult(MessageEntity message, Message _)
  {
    if (message.Result == null)
    {
      return Enumerable.Empty<ResultData>();
    }

    SendMessageResult result = JsonSerializer.Deserialize<SendMessageResult>(message.Result)
      ?? throw new InvalidOperationException($"The result deserialization failed: '{message.Result}'.");

    return result.Select(pair => new ResultData
    {
      Key = pair.Key,
      Value = pair.Value
    });
  }
}
