import type { CreateRealmPayload, SearchRealmsPayload, UpdateRealmPayload } from "@/types/realms/payloads";
import type { Realm } from "@/types/realms";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, patch, post } from ".";

export async function createRealm(payload: CreateRealmPayload): Promise<Realm> {
  return (await post<CreateRealmPayload, Realm>("/api/realms", payload)).data;
}

export async function deleteRealm(id: string): Promise<Realm> {
  return (await _delete<Realm>(`/api/realms/${id}`)).data;
}

export async function readRealm(id: string): Promise<Realm> {
  return (await get<Realm>(`/api/realms/${id}`)).data;
}

export async function readRealmByUniqueSlug(uniqueSlug: string): Promise<Realm> {
  return (await get<Realm>(`/api/realms/unique-slug:${uniqueSlug}`)).data;
}

const searchRealmsQuery = `
query($payload: SearchRealmsPayload!) {
  realms(payload: $payload) {
    results {
      id
      uniqueSlug
      displayName
      uniqueNameSettings {
        allowedCharacters
      }
      passwordSettings {
        requiredLength
        requiredUniqueChars
        requireNonAlphanumeric
        requireLowercase
        requireUppercase
        requireDigit
        strategy
      }
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
type SearchRealmsRequest = {
  payload: SearchRealmsPayload;
};
type SearchRealmsResponse = {
  realms: SearchResults<Realm>;
};
export async function searchRealms(payload: SearchRealmsPayload): Promise<SearchResults<Realm>> {
  return (await graphQL<SearchRealmsRequest, SearchRealmsResponse>(searchRealmsQuery, { payload })).data.realms;
}

export async function updateRealm(id: string, payload: UpdateRealmPayload): Promise<Realm> {
  return (await patch<UpdateRealmPayload, Realm>(`/api/realms/${id}`, payload)).data;
}
