using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Events;

public record DictionaryUpdated : DictionarySaved, INotification;
