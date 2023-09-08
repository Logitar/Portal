import type { SearchResults } from "@/types/search";
import type { SearchSessionsPayload, Session } from "@/types/sessions";
import { get, graphQL, patch } from ".";

export async function readSession(id: string): Promise<Session> {
  return (await get<Session>(`/api/sessions/${id}`)).data;
}

const searchSessionsQuery = `
query($payload: SearchSessionsPayload!) {
  sessions(payload: $payload) {
    results {
      id
      isPersistent
      isActive
      signedOutBy {
        id
        type
        isDeleted
        displayName
        emailAddress
        pictureUrl
      }
      signedOutOn
      updatedOn
      user {
        id
        uniqueName
        email {
          address
        }
        fullName
        picture
      }
    }
    total
  }
}
`;
type SearchSessionsRequest = {
  payload: SearchSessionsPayload;
};
type SearchSessionsResponse = {
  sessions: SearchResults<Session>;
};
export async function searchSessions(payload: SearchSessionsPayload): Promise<SearchResults<Session>> {
  return (await graphQL<SearchSessionsRequest, SearchSessionsResponse>(searchSessionsQuery, { payload })).data.sessions;
}

export async function signOut(id: string): Promise<Session> {
  return (await patch<void, Session>(`/api/sessions/${id}/sign/out`)).data;
}
