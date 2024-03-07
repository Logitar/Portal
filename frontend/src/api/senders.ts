import type { CreateSenderPayload, ReplaceSenderPayload, Sender, SearchSendersPayload } from "@/types/senders";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, patch, post, put } from ".";

export async function createSender(payload: CreateSenderPayload): Promise<Sender> {
  return (await post<CreateSenderPayload, Sender>("/api/senders", payload)).data;
}

export async function deleteSender(id: string): Promise<Sender> {
  return (await _delete<Sender>(`/api/senders/${id}`)).data;
}

export async function readSender(id: string): Promise<Sender> {
  return (await get<Sender>(`/api/senders/${id}`)).data;
}

export async function replaceSender(id: string, payload: ReplaceSenderPayload, version?: number): Promise<Sender> {
  const query: string = version ? `?version=${version}` : "";
  return (await put<ReplaceSenderPayload, Sender>(`/api/senders/${id}${query}`, payload)).data;
}

const searchSendersQuery = `
query($payload: SearchSendersPayload!) {
  senders(payload: $payload) {
    items {
      id
      isDefault
      emailAddress
      displayName
      provider
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
type SearchSendersRequest = {
  payload: SearchSendersPayload;
};
type SearchSendersResponse = {
  senders: SearchResults<Sender>;
};
export async function searchSenders(payload: SearchSendersPayload): Promise<SearchResults<Sender>> {
  return (await graphQL<SearchSendersRequest, SearchSendersResponse>(searchSendersQuery, { payload })).data.senders;
}

export async function setDefaultSender(id: string): Promise<Sender> {
  return (await patch<void, Sender>(`/api/senders/${id}/default`)).data;
}
