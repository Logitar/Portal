import type { Message, SearchMessagesPayload, SendMessagePayload, SentMessages } from "@/types/messages";
import type { SearchResults } from "@/types/search";
import { get, graphQL, post } from ".";

export async function readMessage(id: string): Promise<Message> {
  return (await get<Message>(`/api/messages/${id}`)).data;
}

const searchMessagesQuery = `
query($payload: SearchMessagesPayload!) {
  messages(payload: $payload) {
    items {
      id
      subject
      recipientCount
      sender {
        id
        isDefault
        emailAddress
        displayName
        version
      }
      template {
        id
        uniqueKey
        displayName
        version
      }
      status
      updatedBy {
        id
        type
        isDeleted
        displayName
        emailAddress
        pictureUrl
      }
      updatedOn
    }
    total
  }
}
`;
type SearchMessagesRequest = {
  payload: SearchMessagesPayload;
};
type SearchMessagesResponse = {
  messages: SearchResults<Message>;
};
export async function searchMessages(payload: SearchMessagesPayload): Promise<SearchResults<Message>> {
  return (await graphQL<SearchMessagesRequest, SearchMessagesResponse>(searchMessagesQuery, { payload })).data.messages;
}

export async function sendMessage(payload: SendMessagePayload): Promise<SentMessages> {
  return (await post<SendMessagePayload, SentMessages>("/api/messages", payload)).data;
}
