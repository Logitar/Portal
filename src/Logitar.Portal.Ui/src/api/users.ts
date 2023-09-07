import type { CreateUserPayload, SearchUsersPayload, UpdateUserPayload } from "@/types/users/payloads";
import type { User } from "@/types/users";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, patch, post } from ".";

export async function createUser(payload: CreateUserPayload): Promise<User> {
  return (await post<CreateUserPayload, User>("/api/users", payload)).data;
}

export async function deleteUser(id: string): Promise<User> {
  return (await _delete<User>(`/api/users/${id}`)).data;
}

export async function readUser(id: string): Promise<User> {
  return (await get<User>(`/api/users/${id}`)).data;
}

const searchUsersQuery = `
query($payload: SearchUsersPayload!) {
  users(payload: $payload) {
    results {
      id
      uniqueName
      isDisabled
      authenticatedOn
      email {
        address
        isVerified
      }
      phone {
        e164Formatted
        isVerified
      }
      fullName
      picture
      realm {
        id
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
type SearchUsersRequest = {
  payload: SearchUsersPayload;
};
type SearchUsersResponse = {
  users: SearchResults<User>;
};
export async function searchUsers(payload: SearchUsersPayload): Promise<SearchResults<User>> {
  return (await graphQL<SearchUsersRequest, SearchUsersResponse>(searchUsersQuery, { payload })).data.users;
}

export async function signOut(id: string): Promise<User> {
  return (await patch<void, User>(`/api/users/${id}/sign/out`)).data;
}

export async function updateUser(id: string, payload: UpdateUserPayload): Promise<User> {
  return (await patch<UpdateUserPayload, User>(`/api/users/${id}`, payload)).data;
}
