import type { SearchParameters, SearchResults } from "@/types/api";
import type { CreateRealmPayload, UpdateRealmPayload } from "@/types/realms/payloads";
import type { Realm } from "@/types/realms";
import { _delete, get, graphQL, post, put } from ".";

export async function createRealm(payload: CreateRealmPayload): Promise<Realm> {
  return (await post<CreateRealmPayload, Realm>("/api/realms", payload)).data;
}

export async function deleteRealm(id: string): Promise<Realm> {
  return (await _delete<Realm>(`/api/realms/${id}`)).data;
}

export async function readRealm(uniqueSlug: string): Promise<Realm> {
  return (await get<Realm>(`/api/realms/unique-slug:${uniqueSlug}`)).data;
}

const searchRealmsQuery = `
query($parameters: RealmSearchParameters!) {
  realms(parameters: $parameters) {
    results {
      id
      uniqueSlug
      displayName
      updatedBy {
        id
        type
        isDeleted
        displayName
        emailAddress
        picture
      }
      updatedOn
    }
    total
  }
}
`;
type SearchRealmsRequest = {
  parameters: SearchParameters;
};
type SearchRealmsResponse = {
  realms: SearchResults<Realm>;
};
export async function searchRealms(parameters: SearchParameters): Promise<SearchResults<Realm>> {
  return (await graphQL<SearchRealmsRequest, SearchRealmsResponse>(searchRealmsQuery, { parameters })).data.realms;
}

export async function updateRealm(id: string, payload: UpdateRealmPayload): Promise<Realm> {
  return (await put<UpdateRealmPayload, Realm>(`/api/realms/${id}`, payload)).data;
}
