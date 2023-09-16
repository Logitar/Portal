import type { CreateDictionaryPayload, Dictionary, SearchDictionariesPayload, UpdateDictionaryPayload } from "@/types/dictionaries";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, patch, post } from ".";

export async function createDictionary(payload: CreateDictionaryPayload): Promise<Dictionary> {
  return (await post<CreateDictionaryPayload, Dictionary>("/api/dictionaries", payload)).data;
}

export async function deleteDictionary(id: string): Promise<Dictionary> {
  return (await _delete<Dictionary>(`/api/dictionaries/${id}`)).data;
}

export async function readDictionary(id: string): Promise<Dictionary> {
  return (await get<Dictionary>(`/api/dictionaries/${id}`)).data;
}

const searchDictionariesQuery = `
query($payload: SearchDictionariesPayload!) {
  dictionaries(payload: $payload) {
    results {
      id
      realm {
        uniqueSlug
        displayName
      }
      locale {
        nativeName
      }
      entryCount
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
type SearchDictionariesRequest = {
  payload: SearchDictionariesPayload;
};
type SearchDictionariesResponse = {
  dictionaries: SearchResults<Dictionary>;
};
export async function searchDictionaries(payload: SearchDictionariesPayload): Promise<SearchResults<Dictionary>> {
  return (await graphQL<SearchDictionariesRequest, SearchDictionariesResponse>(searchDictionariesQuery, { payload })).data.dictionaries;
}

export async function updateDictionary(id: string, payload: UpdateDictionaryPayload): Promise<Dictionary> {
  return (await patch<UpdateDictionaryPayload, Dictionary>(`/api/dictionaries/${id}`, payload)).data;
}
