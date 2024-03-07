import type { CreateRolePayload, ReplaceRolePayload, Role, SearchRolesPayload } from "@/types/roles";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, post, put } from ".";

export async function createRole(payload: CreateRolePayload): Promise<Role> {
  return (await post<CreateRolePayload, Role>("/api/roles", payload)).data;
}

export async function deleteRole(id: string): Promise<Role> {
  return (await _delete<Role>(`/api/roles/${id}`)).data;
}

const listRolesQuery = `
query($payload: SearchRolesPayload!) {
  roles(payload: $payload) {
    items {
      id
      uniqueName
      displayName
    }
  }
}
`;
type ListRolesRequest = {
  payload: SearchRolesPayload;
};
type ListRolesResponse = {
  roles: SearchResults<Role>;
};
export async function listRoles(): Promise<Role[]> {
  return (await graphQL<ListRolesRequest, ListRolesResponse>(listRolesQuery, { payload: {} })).data.roles.items;
}

export async function readRole(id: string): Promise<Role> {
  return (await get<Role>(`/api/roles/${id}`)).data;
}

export async function replaceRole(id: string, payload: ReplaceRolePayload, version?: number): Promise<Role> {
  const query: string = version ? `?version=${version}` : "";
  return (await put<ReplaceRolePayload, Role>(`/api/roles/${id}${query}`, payload)).data;
}

const searchRolesQuery = `
query($payload: SearchRolesPayload!) {
  roles(payload: $payload) {
    items {
      id
      uniqueName
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
type SearchRolesRequest = {
  payload: SearchRolesPayload;
};
type SearchRolesResponse = {
  roles: SearchResults<Role>;
};
export async function searchRoles(payload: SearchRolesPayload): Promise<SearchResults<Role>> {
  return (await graphQL<SearchRolesRequest, SearchRolesResponse>(searchRolesQuery, { payload })).data.roles;
}
