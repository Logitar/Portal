using MediatR;

namespace Logitar.Portal.v2.Core.Dictionaries.Events;

public record DictionaryUpdated : DictionarySaved, INotification;
