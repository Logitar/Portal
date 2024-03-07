import type { CreateRealmPayload, Realm, ReplaceRealmPayload, SearchRealmsPayload } from "@/types/realms";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, post, put } from ".";

export async function createRealm(payload: CreateRealmPayload): Promise<Realm> {
  return (await post<CreateRealmPayload, Realm>("/api/realms", payload)).data;
}

export async function deleteRealm(id: string): Promise<Realm> {
  return (await _delete<Realm>(`/api/realms/${id}`)).data;
}

const listRealmsQuery = `
query($payload: SearchRealmsPayload!) {
  realms(payload: $payload) {
    items {
      id
      uniqueSlug
      displayName
    }
  }
}
`;
type ListRealmsRequest = {
  payload: SearchRealmsPayload;
};
type ListRealmsResponse = {
  realms: SearchResults<Realm>;
};
export async function listRealms(): Promise<Realm[]> {
  return (await graphQL<ListRealmsRequest, ListRealmsResponse>(listRealmsQuery, { payload: {} })).data.realms.items;
}

export async function readRealm(id: string): Promise<Realm> {
  return (await get<Realm>(`/api/realms/${id}`)).data;
}

export async function readRealmByUniqueSlug(uniqueSlug: string): Promise<Realm> {
  return (await get<Realm>(`/api/realms/unique-slug:${uniqueSlug}`)).data;
}

export async function replaceRealm(id: string, payload: ReplaceRealmPayload, version?: number): Promise<Realm> {
  const query: string = version ? `?version=${version}` : "";
  return (await put<ReplaceRealmPayload, Realm>(`/api/realms/${id}${query}`, payload)).data;
}

const searchRealmsQuery = `
query($payload: SearchRealmsPayload!) {
  realms(payload: $payload) {
    items {
      id
      uniqueSlug
      displayName
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
