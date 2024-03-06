import type { CreateDictionaryPayload, Dictionary, ReplaceDictionaryPayload, SearchDictionariesPayload } from "@/types/dictionaries";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, post, put } from ".";

export async function createDictionary(payload: CreateDictionaryPayload): Promise<Dictionary> {
  return (await post<CreateDictionaryPayload, Dictionary>("/api/dictionaries", payload)).data;
}

export async function deleteDictionary(id: string): Promise<Dictionary> {
  return (await _delete<Dictionary>(`/api/dictionaries/${id}`)).data;
}

export async function readDictionary(id: string): Promise<Dictionary> {
  return (await get<Dictionary>(`/api/dictionaries/${id}`)).data;
}

export async function replaceDictionary(id: string, payload: ReplaceDictionaryPayload, version?: number): Promise<Dictionary> {
  const query: string = version ? `?version=${version}` : "";
  return (await put<ReplaceDictionaryPayload, Dictionary>(`/api/dictionaries/${id}${query}`, payload)).data;
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
