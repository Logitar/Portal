import type { ApiKey } from "@/types/apiKeys";
import type { CreateApiKeyPayload, SearchApiKeysPayload, UpdateApiKeyPayload } from "@/types/apiKeys/payloads";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, patch, post } from ".";

export async function createApiKey(payload: CreateApiKeyPayload): Promise<ApiKey> {
  return (await post<CreateApiKeyPayload, ApiKey>("/api/keys", payload)).data;
}

export async function deleteApiKey(id: string): Promise<ApiKey> {
  return (await _delete<ApiKey>(`/api/keys/${id}`)).data;
}

export async function readApiKey(id: string): Promise<ApiKey> {
  return (await get<ApiKey>(`/api/keys/${id}`)).data;
}

const searchApiKeysQuery = `
query($payload: SearchApiKeysPayload!) {
  apiKeys(payload: $payload) {
    results {
      id
      displayName
      expiresOn
      authenticatedOn
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
type SearchApiKeysRequest = {
  payload: SearchApiKeysPayload;
};
type SearchApiKeysResponse = {
  apiKeys: SearchResults<ApiKey>;
};
export async function searchApiKeys(payload: SearchApiKeysPayload): Promise<SearchResults<ApiKey>> {
  return (await graphQL<SearchApiKeysRequest, SearchApiKeysResponse>(searchApiKeysQuery, { payload })).data.apiKeys;
}

export async function updateApiKey(id: string, payload: UpdateApiKeyPayload): Promise<ApiKey> {
  return (await patch<UpdateApiKeyPayload, ApiKey>(`/api/keys/${id}`, payload)).data;
}
