import type { Role } from "@/types/roles";
import type { SearchResults } from "@/types/search";
import type { SearchRolesPayload } from "@/types/roles/payloads";
import { _delete, graphQL } from ".";

export async function deleteRole(id: string): Promise<Role> {
  return (await _delete<Role>(`/api/roles/${id}`)).data;
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
