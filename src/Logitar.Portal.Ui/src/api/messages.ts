import type { Message, SearchMessagesPayload, SendDemoMessagePayload } from "@/types/messages";
import type { SearchResults } from "@/types/search";
import { get, graphQL, post } from ".";

export async function readMessage(id: string): Promise<Message> {
  return (await get<Message>(`/api/messages/${id}`)).data;
}

const searchMessagesQuery = `
query($payload: SearchMessagesPayload!) {
  messages(payload: $payload) {
    results {
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
        uniqueName
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

export async function sendDemoMessage(payload: SendDemoMessagePayload): Promise<Message> {
  return (await post<SendDemoMessagePayload, Message>("/api/messages/send/demo", payload)).data;
}
