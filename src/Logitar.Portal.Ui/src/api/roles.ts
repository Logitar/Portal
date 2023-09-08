import type { CreateRolePayload, Role, SearchRolesPayload, UpdateRolePayload } from "@/types/roles";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, patch, post } from ".";

export async function createRole(payload: CreateRolePayload): Promise<Role> {
  return (await post<CreateRolePayload, Role>("/api/roles", payload)).data;
}

export async function deleteRole(id: string): Promise<Role> {
  return (await _delete<Role>(`/api/roles/${id}`)).data;
}

export async function readRole(id: string): Promise<Role> {
  return (await get<Role>(`/api/roles/${id}`)).data;
}

const searchRolesQuery = `
query($payload: SearchRolesPayload!) {
  roles(payload: $payload) {
    results {
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

export async function updateRole(id: string, payload: UpdateRolePayload): Promise<Role> {
  return (await patch<UpdateRolePayload, Role>(`/api/roles/${id}`, payload)).data;
}
