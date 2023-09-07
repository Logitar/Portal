import type { SearchResults } from "@/types/search";
import type { SearchSessionsPayload } from "@/types/sessions/payloads";
import type { Session } from "@/types/sessions";
import { get, graphQL } from ".";

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
