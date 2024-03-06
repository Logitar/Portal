import type { CreateSenderPayload, Sender, SearchSendersPayload, UpdateSenderPayload } from "@/types/senders";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, patch, post } from ".";

export async function createSender(payload: CreateSenderPayload): Promise<Sender> {
  return (await post<CreateSenderPayload, Sender>("/api/senders", payload)).data;
}

export async function deleteSender(id: string): Promise<Sender> {
  return (await _delete<Sender>(`/api/senders/${id}`)).data;
}

export async function readSender(id: string): Promise<Sender> {
  return (await get<Sender>(`/api/senders/${id}`)).data;
}

const searchSendersQuery = `
query($payload: SearchSendersPayload!) {
  senders(payload: $payload) {
    results {
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

export async function updateSender(id: string, payload: UpdateSenderPayload): Promise<Sender> {
  return (await patch<UpdateSenderPayload, Sender>(`/api/senders/${id}`, payload)).data;
}
